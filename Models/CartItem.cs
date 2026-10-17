using Shahbazi.Store.Common.ResultPattern;

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
            return Result<CartItem>.BadRequestError("CartId is Invalid !!");
        }
        if (productId <= 0)
        {
            return Result<CartItem>.BadRequestError("ProductId is Invalid !!");
        }
        if (quantity <= 0)
        {
            return Result<CartItem>.BadRequestError("Quantity must be more !!");
        }
        if (unitPrice <= 0)
        {
            return Result<CartItem>.BadRequestError("Price Can not be 0 !!");
        }
        return Result<CartItem>.Success(
            new CartItem(cartId, productId, quantity, unitPrice));
    }
    public Result IncreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            return Result.BadRequestError("The Amount must be more than 0 !!");
        }
        Quantity += amount;
        return Result.Success();
    }
    public Result DecreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            return Result.BadRequestError("The Amount must be more than 0 !!");
        }
        if (Quantity - amount < 1)
        {
            return Result.BadRequestError("The Quantity Of Product Can not be less than 1 !!");
        }
        Quantity -= amount;
        return Result.Success();
    }
    public Result ChangeQuantity(int quantity)
    {
        if (quantity < 1)
        {
            return Result.BadRequestError("Quantity AtLeast most be 1!!");
        }
        Quantity = quantity;
        return Result.Success();
    }
}