using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Enums;

namespace Shahbazi.Store.Models;

public class Order
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }
    public string ShippingAddress { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public Order(int userId, string shippingAddress)
    {
        UserId = userId;
        ShippingAddress = shippingAddress;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }
    public static Result<Order> Create(int userId, string shippingAddress)
    {
        if (userId <= 0)
        {
            Result<Order>.InvalidId("User");
        }
        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            Result<Order>.BadRequestError("Shipping address is required.");
        }
        return Result<Order>.Success(new Order(userId, shippingAddress));
    }
    public Result AddItem(OrderItem item)
    {
        if (item == null)
        {
            Result.BadRequestError("Order item is invalid.");
        }
        OrderItems.Add(item);
        return Result.Success();
    }
    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        [OrderStatus.Pending] = [OrderStatus.Paid],
        [OrderStatus.Paid] = [OrderStatus.Processing],
        [OrderStatus.Processing] = [OrderStatus.Shipped],
        [OrderStatus.Shipped] = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = [],
        [OrderStatus.Cancelled] = []
    };

    public Result ChangeOrderStatus(OrderStatus status)
    {
        if (!AllowedTransitions[Status].Contains(status))
        {
            Result.ConflictError($"Cannot change order status from {Status} to {status}.");
        }

        Status = status;
        return Result.Success();
    }
    public Result Cancel()
    {
        if (Status == OrderStatus.Cancelled)
        {
            Result.ConflictError("Order is already cancelled.");
        }

        if (Status == OrderStatus.Delivered)
        {
            Result.ConflictError("A delivered order cannot be cancelled.");
        }

        Status = OrderStatus.Cancelled;
        return Result.Success();
    }

    public Result UpdateShippingAddress(string shippingAddress)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            Result.BadRequestError("Shipping address is required.");
        }

        if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Cancelled)
        {
            Result.ConflictError("The address of this order can no longer be changed.");
        }

        ShippingAddress = shippingAddress;
        return Result.Success();
    }
}