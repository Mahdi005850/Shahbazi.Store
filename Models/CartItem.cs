using System.Text.Json.Serialization;
namespace Shahbazi.Store.Models;

public class CartItem
{
    public int Id { get; private set; }
    public int CartId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    [JsonIgnore]public Cart Cart { get; private set; }
    public Product Product { get; private set; } = null;
    public decimal TotalPrice => UnitPrice * Quantity;
    public CartItem(int cartId, int productId, int quantity, decimal unitPrice)
    {
        if (cartId <= 0)
        {
            throw new ArgumentException("CartId is Invalid !!");
        }
        if (productId <= 0)
        {
            throw new ArgumentException("ProductId is Invalid !!");
        }
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be more !!");
        }
        if (unitPrice <= 0)
        {
            throw new ArgumentException("Price Can not be 0 !!");
        }
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("The Amount must be more than 0 !!");
        }
        Quantity += amount;
    }
    public void DecreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("The Amount must be more than 0 !!");
        }
        if (Quantity - amount < 1)
        {
            throw new InvalidOperationException("The Quantity Of Product Can not be less than 1 !!");
        }
        Quantity -= amount;
    }
    public void ChangeQuantity(int quantity)
    {
        if (quantity < 1)
        {
            throw new ArgumentException("Quantity AtLeast most be 1!!");
        }
        Quantity = quantity;
    }
}