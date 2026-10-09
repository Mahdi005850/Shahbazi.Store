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
            return Result<OrderItem>.Failure("OrderId Cannot be 0 !!", ResultErrorType.BadRequest);
        }
        if (productId <= 0)
        {
            return Result<OrderItem>.Failure("ProductId Cannot be 0 !!", ResultErrorType.BadRequest);
        }
        if (quantity <= 0)
        {
            return Result<OrderItem>.Failure("Quantity mmust be more than 0 !!", ResultErrorType.BadRequest);
        }
        if (unitPrice <= 0)
        {
            return Result<OrderItem>.Failure("Price Cannot be under 0 !!", ResultErrorType.BadRequest);
        }
        return Result<OrderItem>.Success(new OrderItem(orderId, productId, quantity, unitPrice));
    }
}