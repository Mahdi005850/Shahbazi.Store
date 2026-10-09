using Shahbazi.Store.Common.Enums;
using Shahbazi.Store.Common.ResultPattern;

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
        CreatedAt = DateTime.Now;
        Status = OrderStatus.Pending;
    }
    public static Result<Order> Create(int userId, string shippingAddress)
    {
        if (userId <= 0)
        {
            return Result<Order>.Failure("Invalid UserId !!", ResultErrorType.BadRequest);
        }
        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            return Result<Order>.Failure("Address Cannot be Empty , Please Enter your address !!", ResultErrorType.BadRequest);
        }
        return Result<Order>.Success(new Order(userId, shippingAddress));
    }
    public Result AddItem(OrderItem item)
    {
        if (item == null)
        {
            return Result.Failure("OrderItem is invalid !!", ResultErrorType.BadRequest);
        }
        OrderItems.Add(item);
        return Result.Success();
    }
    public Result ChangeOrderStatus(OrderStatus status)
    {
        Status = status;
        return Result.Success();
    }
    public Result Cancel()
    {
        if (Status == OrderStatus.Delivered)
        {
            return Result.Failure("Can not Cancel Delivered item !!", ResultErrorType.Conflict);
        }
        Status = OrderStatus.Cancelled;
        return Result.Success();
    }
}