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
            return Result<Product>.BadRequestError("The productName id neccesary!!");
        }
        if (price < 0)
        {
            return Result<Product>.BadRequestError("The price can not be negative !!");
        }
        if (stock < 0)
        {
            return Result<Product>.BadRequestError("Stock Can not be negative !!");
        }
        return Result<Product>.Success(new Product(name, price, description, stock));
    }
    public Result IncreaseStock(int amount)
    {
        if (amount <= 0)
        {
            return Result.BadRequestError("The amount must be more than 0 !!");
        }
        Stock += amount;
        return Result.Success();
    }
    public Result DecreaseStock(int amount)
    {
        if (amount <= 0)
        {
            return Result.BadRequestError("The amount must be more than 0!!");
        }
        if (Stock < amount)
        {
            return Result.ConflictError("No Enough Stock!!");
        }
        Stock -= amount;
        return Result.Success();
    }
    public Result ChangePrice(decimal newPrice)
    {
        if (newPrice < 0)
        {
            return Result.BadRequestError("Price can not be 0 !!");
        }
        Price = newPrice;
        return Result.Success();
    }
    public Result UpdateInformation(string name, decimal price, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.BadRequestError("The productName Cannot be empty !!");
        }
        if (price < 0)
        {
            return Result.BadRequestError("The price Can not be negative !!");
        }
        Name = name;
        Price = price;
        Description = description;
        return Result.Success();
    }
}