using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Data;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;
namespace Shahbazi.Store.Services;

public class OrderServices
{
    private readonly AppDbContext _context;
    private readonly ProductServices _productServices;
    private readonly CartServices _cartServices;
    public OrderServices(
        AppDbContext context,
        CartServices cartServices,
        ProductServices productServices)
    {
        _context = context;
        _cartServices = cartServices;
        _productServices = productServices;
    }
    public Order? GetOrder(int orderId)
    {
        return _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefault(o => o.Id == orderId);
    }
    public Order CreateOrder(int userId, string shippingAddress)
    {
        var cart = _cartServices.GetCart(userId);

        if (cart == null)
        {
            throw new InvalidOperationException("Cart didn't find !!");
        }

        if (!cart.CartItems.Any())
        {
            throw new InvalidOperationException("Cart is Empty !!");
        }
        foreach (var cartItem in cart.CartItems)
        {
            var product = _productServices.GetProduct(cartItem.ProductId);

            if (product == null)
            {
                throw new InvalidOperationException("Product didn't find !!");
            }

            if (cartItem.Quantity > product.Stock)
            {
                throw new InvalidOperationException("Not enough Stock !!");
            }
        }
        var order = new Order(userId, shippingAddress);
        _context.Orders.Add(order);
        _context.SaveChanges();
        foreach (var cartItem in cart.CartItems)
        {
            var product = _productServices.GetProduct(cartItem.ProductId);
            var orderItem = new OrderItem(
                order.Id,
                product!.Id,
                cartItem.Quantity,
                cartItem.UnitPrice);
            order.AddItem(orderItem);
            product.DecreaseStock(cartItem.Quantity);
        }
        cart.Clear();
        _context.SaveChanges();
        return order;
    }
    public void CancelOrder(int orderId)
    {
        var order = GetOrder(orderId);
        if (order == null)
        {
            throw new InvalidOperationException("Order didn't find !!");
        }
        order.Cancel();
        _context.SaveChanges();
    }
    public List<Order> GetUserOrder(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("userId Is Invalid !!");
        }
        return _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .ToList();
    }
    public void ChangeOrderStatus(int orderId, OrderStatus status)
    {
        var order = GetOrder(orderId);
        if (order == null)
        {
            throw new InvalidOperationException("Order Didn't find !!");
        }
        order.ChangeOrderStatus(status);
        _context.SaveChanges();
    }
}