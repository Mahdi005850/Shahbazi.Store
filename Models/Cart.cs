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
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
    public static Result<Cart> Create(int userId)
    {
        if (userId <= 0)
        {
            return Result<Cart>.BadRequestError("UserId is invalid !!");
        }
        return Result<Cart>.Success(new Cart(userId));
    }
    public Result RemoveItem(CartItem item)
    {
        if (item == null)
        {
            return Result.BadRequestError("CartItem is invalid !!");
        }
        CartItems.Remove(item);
        UpdatedAt = DateTime.Now;
        return Result.Success();
    }
    public Result Clear()
    {
        CartItems.Clear();
        UpdatedAt = DateTime.Now;
        return Result.Success();
    }
    public Result AddItem(CartItem cartItem)
    {
        if (cartItem == null)
        {
            return Result.BadRequestError("CartItem is invalid !!");
        }
        CartItems.Add(cartItem);
        UpdatedAt = DateTime.Now;
        return Result.Success();
    }
}