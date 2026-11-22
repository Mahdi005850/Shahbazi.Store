using Shahbazi.Store.Common.ResultPattern;

namespace Shahbazi.Store.Models;

public class Cart
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ICollection<CartItem> CartItems { get; private set; } = new List<CartItem>();
    public decimal TotalPrice => CartItems.Sum(x => x.UnitPrice * x.Quantity);
    public Cart(int userId)
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public static Result<Cart> Create(int userId)
    {
        if (userId <= 0)
        {
            Result<Cart>.InvalidId("User");
        }
        return Result<Cart>.Success(new Cart(userId));
    }
    public Result RemoveItem(CartItem item)
    {
        if (item == null)
        {
            Result.BadRequestError("Cart item is invalid.");
        }
        CartItems.Remove(item);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
    public Result Clear()
    {
        CartItems.Clear();
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
    public Result AddItem(CartItem cartItem)
    {
        if (cartItem == null)
        {
            Result.BadRequestError("Cart item is invalid.");
        }
        CartItems.Add(cartItem);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}