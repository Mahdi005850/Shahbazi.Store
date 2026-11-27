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

    private OrderItem(int orderId, int productId, int quantity, decimal unitPrice)
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
            return Result<OrderItem>.InvalidId("Order");
        }
        if (productId <= 0)
        {
            return Result<OrderItem>.InvalidId("Product");
        }
        if (quantity <= 0)
        {
            return Result<OrderItem>.BadRequestError("Quantity must be greater than zero.");
        }
        if (unitPrice <= 0)
        {
            return Result<OrderItem>.BadRequestError("Price must be greater than zero.");
        }

        return Result<OrderItem>.Success(new OrderItem(orderId, productId, quantity, unitPrice));
    }
}