using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common;
using Shahbazi.Store.Common.Querying;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;
using System.Linq.Expressions;

namespace Shahbazi.Store.Services;

public class OrderServices
{
    private readonly AppDbContext _context;
    private readonly CartServices _cartServices;

    public OrderServices(AppDbContext context, CartServices cartServices, ProductServices productServices)
    {
        _context = context;
        _cartServices = cartServices;
    }

    public async Task<Result<OrderGetResponse>> GetOrderAsync(int orderId)
    {
        var order = await GetOrderModelAsync(orderId);

        if (!order.IsSuccess)
        {
            return order.ToFailure<OrderGetResponse>();
        }
        return Result<OrderGetResponse>.Success(order.Value!.ToGetResponse());
    }

    public async Task<Result<List<OrderGetAllResponse>>> GetUserOrderAsync(int userId)
    {
        if (userId <= 0)
        {
            return Result<List<OrderGetAllResponse>>.BadRequestError("UserId is invalid !!");
        }

        var orders = await _context.Orders.Include(o => o.OrderItems).AsNoTracking().Where(o => o.UserId == userId).ToListAsync();

        return Result<List<OrderGetAllResponse>>.Success(orders.Select(o => o.ToGetAllResponse()).ToList());
    }

    public async Task<Result<OrderGetResponse>> CreateOrderAsync(int userId, string shippingAddress)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
        {
            return Result<OrderGetResponse>.NotFoundError("User didn't find !!");
        }

        var cartResult = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);

        if (cartResult is null)
        {
            return Result<OrderGetResponse>.NotFoundError("Cart didn't find !!");
        }

        var cart = cartResult;

        if (!cart.CartItems.Any())
        {
            return Result<OrderGetResponse>.BadRequestError("Cart is Empty !!");
        }

        foreach (var cartItem in cart.CartItems)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

            if (product is null)
            {
                return Result<OrderGetResponse>.NotFoundError("Product didn't find !!");
            }

            if (cartItem.Quantity > product.Stock)
            {
                return Result<OrderGetResponse>.ConflictError("Not enough stock !!");
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

        return Result<OrderGetResponse>.Success(order.ToGetResponse());
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

        return Result.Success();
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

        return Result.Success();
    }

    private async Task<Result<Order>> GetOrderModelAsync(int orderId)
    {
        if (orderId <= 0)
        {
            return Result<Order>.BadRequestError("Order id is invalid !!");
        }

        var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
        {
            return Result<Order>.NotFoundError("Order didn't find !!");
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

        return Result.Success();
    }
    private static readonly Dictionary<string, Expression<Func<Order, object>>> SortableColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["createdAt"] = o => o.CreatedAt,
        ["status"] = o => o.Status
    };

    public async Task<Result<PagedResult<OrderGetAllResponse>>> GetOrdersAsync(OrderQuery query)
    {
        var validation = query.Validate();

        if (!validation.IsValid)
        {
            return Result<PagedResult<OrderGetAllResponse>>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        if (query.SortBy is not null && !SortableColumns.ContainsKey(query.SortBy))
        {
            return Result<PagedResult<OrderGetAllResponse>>.BadRequestError($"SortBy must be one of: {string.Join(", ", SortableColumns.Keys)}.");
        }

        var orders = _context.Orders.AsNoTracking().Include(o => o.OrderItems).AsQueryable();

        if (query.UserId.HasValue) orders = orders.Where(o => o.UserId == query.UserId.Value);

        if (query.Status.HasValue) orders = orders.Where(o => o.Status == query.Status.Value);

        if (query.FromDate.HasValue) orders = orders.Where(o => o.CreatedAt >= query.FromDate.Value.Date);

        if (query.ToDate.HasValue) orders = orders.Where(o => o.CreatedAt < query.ToDate.Value.Date.AddDays(1));

        var paged = await orders.ApplySort(query.SortBy, query.Descending, SortableColumns, o => o.Id).ToPagedResultAsync(query.Page, query.PageSize, o => o.ToGetAllResponse());

        return Result<PagedResult<OrderGetAllResponse>>.Success(paged);
    }
}