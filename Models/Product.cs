using Shahbazi.Store.ResultPattern;

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
            return Result<Product>.Failure("The productName id neccesary!!", ResultErrorType.BadRequest);
        }
        if (price < 0)
        {
            return Result<Product>.Failure("The price can not be negative !!", ResultErrorType.BadRequest);
        }
        if (stock < 0)
        {
            return Result<Product>.Failure("Stock Can not be negative !!", ResultErrorType.BadRequest);
        }
        return Result<Product>.Success(new Product(name, price, description, stock));
    }
    public Result IncreaseStock(int amount)
    {
        if (amount <= 0)
        {
            return Result.Failure("The amount must be more than 0 !!", ResultErrorType.BadRequest);
        }
        Stock += amount;
        return Result.Success();
    }
    public Result DecreaseStock(int amount)
    {
        if (amount <= 0)
        {
            return Result.Failure("The amount must be more than 0!!", ResultErrorType.BadRequest);
        }
        if (Stock < amount)
        {
            return Result.Failure("No Enough Stock!!", ResultErrorType.Conflict);
        }
        Stock -= amount;
        return Result.Success();
    }
    public Result ChangePrice(decimal newPrice)
    {
        if (newPrice < 0)
        {
            return Result.Failure("Price can not be 0 !!", ResultErrorType.BadRequest);
        }
        Price = newPrice;
        return Result.Success();
    }
    public Result UpdateInformation(string name, decimal price, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure("The productName Cannot be empty !!", ResultErrorType.BadRequest);
        }
        if (price < 0)
        {
            return Result.Failure("The price Can not be negative !!", ResultErrorType.BadRequest);
        }
        Name = name;
        Price = price;
        Description = description;
        return Result.Success();
    }
}