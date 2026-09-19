namespace Shahbazi.Store.Models;

public class Cart
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public User User { get; private set; } = null;
    public ICollection<CartItem> CartItems { get; private set; } = new List<CartItem>();
    public decimal TotalPrice => CartItems.Sum(x => x.UnitPrice * x.Quantity);
    public Cart(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("UserId is invalid !!");
        }
        UserId = userId;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
    public void RemoveItem(CartItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        CartItems.Remove(item);
        UpdatedAt = DateTime.Now;
    }
    public void Clear()
    {
        CartItems.Clear();
        UpdatedAt = DateTime.Now;
    }
    public void AddItem(CartItem cartItem)
    {
        if (cartItem == null)
        {
            throw new ArgumentNullException(nameof(cartItem));
        }
        CartItems.Add(cartItem);
        UpdatedAt = DateTime.Now;
    }
}