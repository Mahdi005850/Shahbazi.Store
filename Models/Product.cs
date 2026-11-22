using Shahbazi.Store.Common.ResultPattern;

namespace Shahbazi.Store.Models;

public class Product
{
    public int Id { get; internal set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public int Stock { get; private set; }
    public Product(string name, decimal price, string? description, int stock)
    {
        Name = name;
        Price = price;
        Description = description;
        Stock = stock;
    }
    public static Result<Product> Create(string name, decimal price, string? description, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Result<Product>.BadRequestError("Name is required.");
        }
        if (price <= 0)
        {
            Result<Product>.BadRequestError("Price must be greater than zero.");
        }
        if (stock < 0)
        {
            Result<Product>.BadRequestError("Stock cannot be negative.");
        }

        return Result<Product>.Success(new Product(name, price, description, stock));
    }
    public Result IncreaseStock(int amount)
    {
        if (amount <= 0)
        {
            Result.BadRequestError("Amount must be greater than zero.");
        }
        Stock += amount;
        return Result.Success();
    }
    public Result DecreaseStock(int amount)
    {
        if (amount <= 0)
        {
            Result.BadRequestError("Amount must be greater than zero.");
        }
        if (Stock < amount)
        {
            Result.ConflictError("Not enough stock for this product.");
        }
        Stock -= amount;
        return Result.Success();
    }
    public Result ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0)
        {
            Result.BadRequestError("Price must be greater than zero.");
        }
        Price = newPrice;
        return Result.Success();
    }
    public Result UpdateInformation(string name, decimal price, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Result.BadRequestError("Name is required.");
        }
        if (price <= 0)
        {
            Result.BadRequestError("Price must be greater than zero.");
        }
        Name = name;
        Price = price;
        Description = description;
        return Result.Success();
    }
}