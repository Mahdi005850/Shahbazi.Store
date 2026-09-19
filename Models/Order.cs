using Shahbazi.Store.Enums;

namespace Shahbazi.Store.Models;

public class Order
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }
    public string ShippingAddress { get; private set; }
    public User User { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    //public long TotalPrice => orderItems.Sum(x => x.TotalPrice);
    public Order(int userId, string shippingAddress)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Invalid UserId !!");
        }
        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            throw new ArgumentException("Address Cannot be Empty , Please Enter your address !!");
        }
        UserId = userId;
        ShippingAddress = shippingAddress;
        CreatedAt = DateTime.Now;
        Status = OrderStatus.Pending;
    }
    public void AddItem(OrderItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        OrderItems.Add(item);
    }
    public void ChangeOrderStatus(OrderStatus status)
    {
        Status = status;
    }
    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Can not Cancel Delivered item !!");
        }
        Status = OrderStatus.Cancelled;
    }
}