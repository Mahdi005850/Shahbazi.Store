using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common.Querying;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.Services;

public class OrderServices
{
    private readonly AppDbContext _context;

    public OrderServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OrderGetResponse>> GetOrderAsync(int orderId)
    {
        var order = await GetOrderModelAsync(orderId);

        if (!order.IsSuccess)
        {
            return order.ToFailure<OrderGetResponse>();
        }
        return Result<OrderGetResponse>.Success(order.Value!.ToGetResponse(), "Order retrieved successfully.");
    }

    public async Task<Result<List<OrderGetAllResponse>>> GetUserOrderAsync(int userId)
    {
        if (userId <= 0)
        {
            return Result<List<OrderGetAllResponse>>.InvalidId("User");
        }

        var orders = await _context.Orders.Include(o => o.OrderItems).AsNoTracking().Where(o => o.UserId == userId).ToListAsync();

        return Result<List<OrderGetAllResponse>>.Success(orders.Select(o => o.ToGetAllResponse()).ToList(), "User orders retrieved successfully.");
    }

    public async Task<Result<OrderGetResponse>> CreateOrderAsync(int userId, string shippingAddress)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
        {
            return Result<OrderGetResponse>.NotFound("User");
        }

        var cartResult = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);

        if (cartResult is null)
        {
            return Result<OrderGetResponse>.NotFound("Cart");
        }

        var cart = cartResult;

        if (!cart.CartItems.Any())
        {
            return Result<OrderGetResponse>.BadRequestError("Cart is empty.");
        }

        foreach (var cartItem in cart.CartItems)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

            if (product is null)
            {
                return Result<OrderGetResponse>.NotFound("Product");
            }

            if (cartItem.Quantity > product.Stock)
            {
                return Result<OrderGetResponse>.ConflictError("Not enough stock for one or more products.");
            }
        }

        var orderResult = Order.Create(userId, shippingAddress);

        if (!orderResult.IsSuccess)
        {
            return orderResult.ToFailure<OrderGetResponse>();
        }

        var order = orderResult.Value!;

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        foreach (var cartItem in cart.CartItems.ToList())
        {
            var product = await _context.Products.FirstAsync(p => p.Id == cartItem.ProductId);

            var orderItemResult = OrderItem.Create(order.Id, product.Id, cartItem.Quantity, cartItem.UnitPrice);

            if (!orderItemResult.IsSuccess)
            {
                return orderItemResult.ToFailure<OrderGetResponse>();
            }
            var addItemResult = order.AddItem(orderItemResult.Value!);

            if (!addItemResult.IsSuccess)
            {
                return addItemResult.ToFailure<OrderGetResponse>();
            }
            var decreaseResult = product.DecreaseStock(cartItem.Quantity);

            if (!decreaseResult.IsSuccess)
            {
                return decreaseResult.ToFailure<OrderGetResponse>();
            }
        }

        cart.Clear();

        await _context.SaveChangesAsync();

        return Result<OrderGetResponse>.Success(order.ToGetResponse(), "Order created successfully.");
    }

    public async Task<Result> CancelOrderAsync(int orderId)
    {
        var validation = new OrderDeleteRequest(orderId).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var orderResult = await GetOrderModelAsync(orderId);

        if (!orderResult.IsSuccess)
        {
            return orderResult;
        }
        var result = orderResult.Value!.Cancel();

        if (!result.IsSuccess) return result;

        foreach (var item in orderResult.Value!.OrderItems)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
            product?.IncreaseStock(item.Quantity);
        }

        await _context.SaveChangesAsync();

        return Result.Success("Order cancelled successfully.");
    }

    public async Task<Result> ChangeOrderStatusAsync(int orderId, OrderStatus status)
    {
        var orderResult = await GetOrderModelAsync(orderId);

        if (!orderResult.IsSuccess)
        {
            return orderResult;
        }
        var result = orderResult.Value!.ChangeOrderStatus(status);

        if (!result.IsSuccess)
        {
            return result;
        }

        await _context.SaveChangesAsync();

        return Result.Success("Order status updated successfully.");
    }

    private async Task<Result<Order>> GetOrderModelAsync(int orderId)
    {
        if (orderId <= 0)
        {
            return Result<Order>.InvalidId("Order");
        }

        var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
        {
            return Result<Order>.NotFound("Order");
        }

        return Result<Order>.Success(order);
    }

    public async Task<Result> UpdateOrderAddressAsync(int orderId, string shippingAddress)
    {
        var orderResult = await GetOrderModelAsync(orderId);

        if (!orderResult.IsSuccess)
        {
            return orderResult;
        }

        var validation = new OrderUpdateRequest(shippingAddress).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var result = orderResult.Value!.UpdateShippingAddress(shippingAddress);

        if (!result.IsSuccess)
        {
            return result;
        }

        await _context.SaveChangesAsync();

        return Result.Success("Order address updated successfully.");
    }

    public async Task<Result<List<OrderGetAllResponse>>> GetOrdersAsync(OrderQuery query, int? sellerId = null)
    {
        var validation = query.Validate();

        if (!validation.IsValid)
        {
            return Result<List<OrderGetAllResponse>>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var orders = _context.Orders.AsNoTracking().Include(o => o.OrderItems).AsQueryable();

        if (sellerId.HasValue)
        {
            orders = orders.Where(o => o.OrderItems.Any(i =>
            _context.Products.Any(p => p.Id == i.ProductId && p.SellerId == sellerId.Value)));
        }


        if (query.UserId.HasValue) orders = orders.Where(o => o.UserId == query.UserId.Value);

        if (query.Status.HasValue) orders = orders.Where(o => o.Status == query.Status.Value);

        if (query.FromDate.HasValue) orders = orders.Where(o => o.CreatedAt >= query.FromDate.Value.Date);

        if (query.ToDate.HasValue) orders = orders.Where(o => o.CreatedAt < query.ToDate.Value.Date.AddDays(1));

        var items = await orders
            .OrderBy(o => o.Id)
            .Page(query.Page, query.PageSize)
            .ToGetAllResponses()
            .ToListAsync();

        return Result<List<OrderGetAllResponse>>.Success(items, "Orders retrieved successfully.");
    }

    public Task<int?> GetOrderOwnerIdAsync(int orderId) => _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => (int?)o.UserId)
            .FirstOrDefaultAsync();

    public Task<bool> SellerHasItemInOrderAsync(int orderId, int sellerId) => _context.OrderItems.AnyAsync(i => i.OrderId == orderId && _context.Products.Any(p => p.Id == i.ProductId && p.SellerId == sellerId));
}