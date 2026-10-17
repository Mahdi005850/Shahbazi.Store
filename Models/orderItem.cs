using Shahbazi.Store.Common.ResultPattern;

namespace Shahbazi.Store.Models;

public class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public OrderItem(int orderId, int productId, int quantity, decimal unitPrice)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    public static Result<OrderItem> Create(int orderId, int productId, int quantity, decimal unitPrice)
    {
        if (orderId <= 0)
        {
            return Result<OrderItem>.BadRequestError("OrderId Cannot be 0 !!");
        }
        if (productId <= 0)
        {
            return Result<OrderItem>.BadRequestError("ProductId Cannot be 0 !!");
        }
        if (quantity <= 0)
        {
            return Result<OrderItem>.BadRequestError("Quantity mmust be more than 0 !!");
        }
        if (unitPrice <= 0)
        {
            return Result<OrderItem>.BadRequestError("Price Cannot be under 0 !!");
        }
        return Result<OrderItem>.Success(new OrderItem(orderId, productId, quantity, unitPrice));
    }
}