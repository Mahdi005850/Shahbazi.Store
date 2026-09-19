using System.Text.Json.Serialization;
namespace Shahbazi.Store.Models;

public class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    [JsonIgnore] public Order Order { get; private set; }
    [JsonIgnore] public Product Product { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public OrderItem(int orderId, int productId, int quantity, decimal unitPrice)
    {
        if (orderId <= 0)
        {
            throw new ArgumentException("OrderId Cannot be 0 !!");
        }
        if (productId <= 0)
        {
            throw new ArgumentException("ProductId Cannot be 0 !!");
        }
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity mmust be more than 0 !!");
        }
        if (unitPrice <= 0)
        {
            throw new ArgumentException("Price Cannot be under 0 !!");
        }
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}