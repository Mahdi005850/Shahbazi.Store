namespace Shahbazi.Store.Models;

public class Product
{
    public string ProductName { get; private set; }
    public decimal ProductPrice { get; private set; }
    public string? ProductDescription { get; private set; }
    public int Stock { get; private set; }
    public ICollection<CartItem> CartItems { get; private set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public int Id { get; internal set; }
    public Product(string productName, decimal productPrice, string? productDescription, int stock)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("The productName id neccesary!!");
        }
        if (productPrice < 0)
        {
            throw new ArgumentException("The price can not be negative !!");
        }
        if (stock < 0)
        {
            throw new ArgumentException("Stock Can not be negative !!");
        }
        ProductName = productName;
        ProductPrice = productPrice;
        ProductDescription = productDescription;
        Stock = stock;
    }

    public void IncreaseStock(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("The amount must be more than 0 !!");
        }
        Stock += amount;
    }
    public void DecreaseStock(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("The amount must be more than 0!!");
        }
        if (Stock < amount)
        {
            throw new InvalidOperationException("No Enough Stock!!");
        }
        Stock -= amount;
    }
    public void ChangePrice(decimal newPrice)
    {
        if (newPrice < 0)
        {
            throw new ArgumentException("Price can not be 0 !!");
        }
        ProductPrice = newPrice;
    }
    public void UpdateInformation(string productName, decimal productPrice, string? productDescription)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("The productName Cannot be empty !!");
        }
        if (productPrice < 0)
        {
            throw new ArgumentException("The price Can not be negative !!");
        }
        ProductName = productName;
        ProductPrice = productPrice;
        ProductDescription = productDescription;
    }

}