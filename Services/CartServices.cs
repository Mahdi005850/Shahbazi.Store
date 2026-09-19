using Shahbazi.Store.Models;
using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Data;
namespace Shahbazi.Store.Services;

public class CartServices
{
    private readonly AppDbContext _context;
    private readonly ProductServices _productServices;
    public CartServices(ProductServices productServices, AppDbContext context)
    {
        _productServices = productServices;
        _context = context;
    }
    public Cart? GetCart(int userId)
    {
        return _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);
    }
    public Cart CreatCart(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Invalid UserId !!");
        }
        var existingCart = GetCart(userId);
        if (existingCart != null)
        {
            throw new InvalidOperationException("This User has exsisting Cart !!");
        }
        var cart = new Cart(userId);
        _context.Carts.Add(cart);
        _context.SaveChanges();
        return cart;
    }
    public void AddToCart(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantities must be more than 0 !!");

        var cart = GetCart(userId);

        if (cart == null)
            throw new InvalidOperationException("Cart didn't find !!");

        var product = _productServices.GetProduct(productId);

        if (product == null)
            throw new InvalidOperationException("Product didn't find !!");

        if (quantity > product.Stock)
            throw new InvalidOperationException("Not enough stock !!");

        var existingItem = cart.CartItems
            .FirstOrDefault(x => x.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            var cartItem = new CartItem(
                cart.Id,
                product.Id,
                quantity,
                product.ProductPrice);
            cart.AddItem(cartItem);
        }
        _context.SaveChanges();
    }
    public void RemoveFromCart(int userId, int productId)
    {
        var cart = GetCart(userId);
        if (cart == null)
        {
            throw new InvalidOperationException("Cart didn't find !!");
        }
        var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem == null)
        {
            throw new InvalidOperationException("Product is not in the cart !!");
        }
        cart.RemoveItem(cartItem);
        _context.SaveChanges();
    }
    public void IncreaseQuantity(int userId, int productId, int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be more than 0 !!");
        }
        var cart = GetCart(userId);
        if (cart == null)
        {
            throw new InvalidOperationException("Cart didn't find !!");
        }
        var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem == null)
        {
            throw new InvalidOperationException("Product is not in the cart !!");
        }
        var product = _productServices.GetProduct(productId);
        if (product == null)
        {
            throw new InvalidOperationException("Product didn't find !!");
        }
        if (cartItem.Quantity + amount > product.Stock)
        {
            throw new InvalidOperationException("Not enough Stock !!");
        }
        cartItem.IncreaseQuantity(amount);
        _context.SaveChanges();
    }
    public void DecreaseQuantity(int userId, int productId, int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be more than 0 !!");
        }
        var cart = GetCart(userId);
        if (cart == null)
        {
            throw new InvalidOperationException("Cart didn't find !!");
        }
        var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem == null)
        {
            throw new InvalidOperationException("Product is not in the cart !!");
        }
        cartItem.DecreaseQuantity(amount);
        _context.SaveChanges();
    }
    public void ClearCart(int userId)
    {
        var cart = GetCart(userId);
        if (cart == null)
        {
            throw new InvalidOperationException("Cart didn't find");
        }
        cart.Clear();
        _context.SaveChanges();
    }
}