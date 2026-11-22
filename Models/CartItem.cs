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
            Result<CartItem>.InvalidId("Cart");
        }
        if (productId <= 0)
        {
            Result<CartItem>.InvalidId("Product");
        }
        if (quantity <= 0)
        {
            Result<CartItem>.BadRequestError("Quantity must be greater than zero.");
        }
        if (unitPrice <= 0)
        {
            Result<CartItem>.BadRequestError("Price must be greater than zero.");
        }
        return Result<CartItem>.Success(
            new CartItem(cartId, productId, quantity, unitPrice));
    }
    public Result IncreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            Result.BadRequestError("Amount must be greater than zero.");
        }
        Quantity += amount;
        return Result.Success();
    }
    public Result DecreaseQuantity(int amount)
    {
        if (amount <= 0)
        {
            Result.BadRequestError("Amount must be greater than zero.");
        }
        if (Quantity - amount < 1)
        {
            Result.BadRequestError("Quantity cannot be less than 1.");
        }
        Quantity -= amount;
        return Result.Success();
    }
    public Result ChangeQuantity(int quantity)
    {
        if (quantity < 1)
        {
            Result.BadRequestError("Quantity must be at least 1.");
        }
        Quantity = quantity;
        return Result.Success();
    }
}