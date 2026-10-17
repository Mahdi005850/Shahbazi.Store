using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;

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

    public Result<OrderGetResponse> GetOrder(int orderId)
    {
        var order = GetOrderModel(orderId);

        if (!order.IsSuccess)
        {
            return order.ToFailure<OrderGetResponse>();
        }
        return Result<OrderGetResponse>.Success(ToGetResponse(order.Value!));
    }

    public Result<List<OrderGetAllResponse>> GetUserOrder(int userId)
    {
        if (userId <= 0)
        {
            return Result<List<OrderGetAllResponse>>.BadRequestError("UserId is invalid !!");
        }

        var orders = _context.Orders.Include(o => o.OrderItems).AsNoTracking().Where(o => o.UserId == userId).ToList();

        return Result<List<OrderGetAllResponse>>.Success(orders.Select(ToGetAllResponse).ToList());
    }

    public Result<OrderGetResponse> CreateOrder(int userId, string shippingAddress)
    {
        if (!_context.Users.Any(u => u.Id == userId))
        {
            return Result<OrderGetResponse>.NotFoundError("User didn't find !!");
        }

        var cartResult = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);

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
            var product = _context.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);

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
        _context.SaveChanges();

        foreach (var cartItem in cart.CartItems.ToList())
        {
            var product = _context.Products.First(p => p.Id == cartItem.ProductId);

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
        _context.SaveChanges();

        return Result<OrderGetResponse>.Success(ToGetResponse(order));
    }

    public Result CancelOrder(int orderId)
    {
        var orderResult = GetOrderModel(orderId);

        if (!orderResult.IsSuccess)
        {
            return orderResult;
        }
        var result = orderResult.Value!.Cancel();

        if (!result.IsSuccess)
        {
            return result;
        }

        _context.SaveChanges();
        return Result.Success();
    }

    public Result ChangeOrderStatus(int orderId, OrderStatus status)
    {
        var orderResult = GetOrderModel(orderId);

        if (!orderResult.IsSuccess)
        {
            return orderResult;
        }
        var result = orderResult.Value!.ChangeOrderStatus(status);

        if (!result.IsSuccess)
        {
            return result;
        }

        _context.SaveChanges();

        return Result.Success();
    }

    private Result<Order> GetOrderModel(int orderId)
    {
        if (orderId <= 0)
        {
            return Result<Order>.BadRequestError("Order id is invalid !!");
        }

        var order = _context.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);

        if (order is null)
        {
            return Result<Order>.NotFoundError("Order didn't find !!");
        }

        return Result<Order>.Success(order);
    }

    private static OrderGetResponse ToGetResponse(Order order) => new(order.Id, order.UserId, order.CreatedAt, order.Status, order.ShippingAddress, order.OrderItems.Sum(item => item.TotalPrice), order.OrderItems.Select(ToOrderItemResponse).ToList());

    private static OrderGetAllResponse ToGetAllResponse(Order order) => new(order.Id, order.UserId, order.CreatedAt, order.Status, order.ShippingAddress, order.OrderItems.Sum(item => item.TotalPrice));

    private static OrderItemGetResponse ToOrderItemResponse(OrderItem item) => new(item.Id, item.ProductId, item.Quantity, item.UnitPrice, item.TotalPrice);
}