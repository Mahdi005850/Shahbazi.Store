using Shahbazi.Store.ResultPattern;

namespace Shahbazi.Store.Models;

public class CartItem
{
    public int Id { get; private set; }
    public int CartId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public CartItem(int cartId, int productId, int quantity, decimal unitPrice)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    public static Result<CartItem> Create(int cartId, int productId, int quantity, decimal unitPrice)
    {
        if (cartId <= 0)
        {
            return Result<CartItem>.Failure("CartId is Invalid !!", ResultErrorType.BadRequest);
        }
        if (productId <= 0)
        {
            return Result<CartItem>.Failure("ProductId is Invalid !!", ResultErrorType.BadRequest);
        }
        if (quantity <= 0)
        {
            return Result<CartItem>.Failure("Quantity must be more !!", ResultErrorType.BadRequest);
        }
        if (unitPrice <= 0)
        {
            return Result<CartItem>.Failure("Price Can not be 0 !!", ResultErrorType.BadRequest);
        }
        return Result<CartItem>.Success(
            new CartItem(cartId, productId, quantity, unitPrice));
    }
    public Result IncreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            return Result.Failure("The Amount must be more than 0 !!", ResultErrorType.BadRequest);
        }
        Quantity += amount;
        return Result.Success();
    }
    public Result DecreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            return Result.Failure("The Amount must be more than 0 !!", ResultErrorType.BadRequest);
        }
        if (Quantity - amount < 1)
        {
            return Result.Failure("The Quantity Of Product Can not be less than 1 !!", ResultErrorType.BadRequest);
        }
        Quantity -= amount;
        return Result.Success();
    }
    public Result ChangeQuantity(int quantity)
    {
        if (quantity < 1)
        {
            return Result.Failure("Quantity AtLeast most be 1!!", ResultErrorType.BadRequest);
        }
        Quantity = quantity;
        return Result.Success();
    }
}