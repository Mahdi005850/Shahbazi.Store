using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Data;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;
using Shahbazi.Store.ResultPattern;

namespace Shahbazi.Store.Services;

public class OrderServices
{
    private readonly AppDbContext _context;
    private readonly ProductServices _productServices;
    private readonly CartServices _cartServices;
    public OrderServices(AppDbContext context, CartServices cartServices, ProductServices productServices)
    {
        _context = context;
        _cartServices = cartServices;
        _productServices = productServices;
    }
    public Result<Order> GetOrder(int orderId)
    {
        var order = _context.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            return Result<Order>.Failure("Order didn't find !!", ResultErrorType.NotFound);
        }
        return Result<Order>.Success(order);
    }
    public Result<Order> CreateOrder(int userId, string shippingAddress)
    {
        var cartResult = _cartServices.GetCart(userId);
        if (!cartResult.IsSuccess)
        {
            return Result<Order>.Failure(cartResult.Error!, cartResult.ErrorType);
        }
        var cart = cartResult.Value!;
        if (!cart.CartItems.Any())
        {
            return Result<Order>.Failure("Cart is Empty !!", ResultErrorType.BadRequest);
        }
        foreach (var cartItem in cart.CartItems)
        {
            var productResult = _productServices.GetProduct(cartItem.ProductId);
            if (!productResult.IsSuccess)
            {
                return Result<Order>.Failure(productResult.Error!, productResult.ErrorType);
            }
            var product = productResult.Value!;
            if (cartItem.Quantity > product.Stock)
            {
                return Result<Order>.Failure("Not enough Stock !!", ResultErrorType.Conflict);
            }
        }
        var orderResult = Order.Create(userId, shippingAddress);
        if (!orderResult.IsSuccess)
        {
            return orderResult;
        }
        var order = orderResult.Value!;
        _context.Orders.Add(order);
        _context.SaveChanges();
        foreach (var cartItem in cart.CartItems)
        {
            var productResult = _productServices.GetProduct(cartItem.ProductId);
            if (!productResult.IsSuccess)
            {
                return Result<Order>.Failure(productResult.Error!, productResult.ErrorType);
            }
            var product = productResult.Value!;
            var orderItemResult = OrderItem.Create(order.Id, product.Id, cartItem.Quantity, cartItem.UnitPrice);
            if (!orderItemResult.IsSuccess)
            {
                return Result<Order>.Failure(orderItemResult.Error!, orderItemResult.ErrorType);
            }
            var addItemResult =
                order.AddItem(orderItemResult.Value!);
            if (!addItemResult.IsSuccess)
            {
                return Result<Order>.Failure(addItemResult.Error!, addItemResult.ErrorType);
            }
            var decreaseResult = product.DecreaseStock(cartItem.Quantity);
            if (!decreaseResult.IsSuccess)
            {
                return Result<Order>.Failure(decreaseResult.Error!, decreaseResult.ErrorType);
            }
        }
        cart.Clear();
        _context.SaveChanges();
        return Result<Order>.Success(order);
    }
    public Result CancelOrder(int orderId)
    {
        var orderResult = GetOrder(orderId);
        if (!orderResult.IsSuccess)
        {
            return Result.Failure(orderResult.Error!, orderResult.ErrorType);
        }
        var result = orderResult.Value!.Cancel();
        if (!result.IsSuccess)
        {
            return result;
        }
        _context.SaveChanges();
        return Result.Success();
    }
    public Result<List<Order>> GetUserOrder(int userId)
    {
        if (userId <= 0)
        {
            return Result<List<Order>>.Failure("userId Is Invalid !!", ResultErrorType.BadRequest);
        }
        return Result<List<Order>>.Success(_context.Orders.Include(o => o.OrderItems).Where(o => o.UserId == userId).ToList());
    }
    public Result ChangeOrderStatus(int orderId, OrderStatus status)
    {
        var orderResult = GetOrder(orderId);
        if (!orderResult.IsSuccess)
        {
            return Result.Failure(orderResult.Error!, orderResult.ErrorType);
        }
        var result = orderResult.Value!.ChangeOrderStatus(status);
        if (!result.IsSuccess)
        {
            return result;
        }
        _context.SaveChanges();
        return Result.Success();
    }
}