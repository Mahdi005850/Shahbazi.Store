using Shahbazi.Store.Common.ResultPattern;

namespace Shahbazi.Store.Models;

public class Product
{
    public int Id { get; internal set; }
    public int SellerId { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public int Stock { get; private set; }

    public Product(int sellerId, string name, decimal price, string? description, int stock)
    {
        SellerId = sellerId;
        Name = name;
        Price = price;
        Description = description;
        Stock = stock;
    }

    public static Result<Product> Create(int sellerId, string name, decimal price, string? description, int stock)
    {
        if (sellerId <= 0)
            return Result<Product>.InvalidId("Seller");
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.BadRequestError("Name is required.");
        if (price <= 0)
            return Result<Product>.BadRequestError("Price must be greater than zero.");
        if (stock < 0)
            return Result<Product>.BadRequestError("Stock cannot be negative.");

        return Result<Product>.Success(new Product(sellerId, name, price, description, stock));
    }

    public Result IncreaseStock(int amount)
    {
        if (amount <= 0)
            return Result.BadRequestError("Amount must be greater than zero.");

        Stock += amount;
        return Result.Success();
    }

    public Result DecreaseStock(int amount)
    {
        if (amount <= 0)
            return Result.BadRequestError("Amount must be greater than zero.");
        if (Stock < amount)
            return Result.ConflictError("Not enough stock for this product.");

        Stock -= amount;
        return Result.Success();
    }

    public Result ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            return Result.BadRequestError("Price must be greater than zero.");

        Price = newPrice;
        return Result.Success();
    }

    public Result UpdateInformation(string name, decimal price, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.BadRequestError("Name is required.");
        if (price <= 0)
            return Result.BadRequestError("Price must be greater than zero.");

        Name = name;
        Price = price;
        Description = description;
        return Result.Success();
    }
}