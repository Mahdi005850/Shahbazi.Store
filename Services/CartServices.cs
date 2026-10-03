using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Data;
using Shahbazi.Store.Models;
using Shahbazi.Store.ResultPattern;

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
    public Result<Cart> GetCart(int userId)
    {
        var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);
        if (cart == null)
        {
            return Result<Cart>.Failure("Cart didn't find !!", ResultErrorType.NotFound);
        }
        return Result<Cart>.Success(cart);
    }
    public Result<Cart> CreatCart(int userId)
    {
        if (userId <= 0)
        {
            return Result<Cart>.Failure("Invalid UserId !!", ResultErrorType.BadRequest);
        }
        var existingCart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
        if (existingCart != null)
        {
            return Result<Cart>.Failure("This User has exsisting Cart !!", ResultErrorType.Conflict);
        }
        var cartResult = Cart.Create(userId);
        if (!cartResult.IsSuccess)
        {
            return cartResult;
        }
        var cart = cartResult.Value!;
        _context.Carts.Add(cart);
        _context.SaveChanges();
        return Result<Cart>.Success(cart);
    }
    public Result AddToCart(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure("Quantities must be more than 0 !!", ResultErrorType.BadRequest);
        }
        var cartResult = GetCart(userId);
        if (!cartResult.IsSuccess)
        {
            return Result.Failure(cartResult.Error!, cartResult.ErrorType);
        }
        var productResult = _productServices.GetProduct(productId);
        if (!productResult.IsSuccess)
        {
            return Result.Failure(productResult.Error!, productResult.ErrorType);
        }
        var cart = cartResult.Value!;
        var product = productResult.Value!;
        if (quantity > product.Stock)
        {
            return Result.Failure("Not enough stock !!", ResultErrorType.Conflict);
        }
        var existingItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            var result = existingItem.IncreaseQuantity(quantity);
            if (!result.IsSuccess)
            {
                return result;
            }
        }
        else
        {
            var cartItemResult = CartItem.Create(cart.Id, product.Id, quantity, product.Price);
            if (!cartItemResult.IsSuccess)
            {
                return Result.Failure(cartItemResult.Error!, cartItemResult.ErrorType);
            }
            var result = cart.AddItem(cartItemResult.Value!);
            if (!result.IsSuccess)
            {
                return result;
            }
        }
        _context.SaveChanges();
        return Result.Success();
    }
    public Result RemoveFromCart(int userId, int productId)
    {
        var cartResult = GetCart(userId);
        if (!cartResult.IsSuccess)
        {
            return Result.Failure(cartResult.Error!, cartResult.ErrorType);
        }
        var cart = cartResult.Value!;
        var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem == null)
        {
            return Result.Failure("Product is not in the cart !!", ResultErrorType.NotFound);
        }
        var result = cart.RemoveItem(cartItem);
        if (!result.IsSuccess)
        {
            return result;
        }
        _context.SaveChanges();
        return Result.Success();
    }
    public Result IncreaseQuantity(int userId, int productId, int amount)
    {
        if (amount <= 0)
        {
            return Result.Failure("Amount must be more than 0 !!", ResultErrorType.BadRequest);
        }
        var cartResult = GetCart(userId);
        if (!cartResult.IsSuccess)
        {
            return Result.Failure(cartResult.Error!, cartResult.ErrorType);
        }
        var cart = cartResult.Value!;
        var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem == null)
        {
            return Result.Failure("Product is not in the cart !!", ResultErrorType.NotFound);
        }
        var productResult = _productServices.GetProduct(productId);
        if (!productResult.IsSuccess)
        {
            return Result.Failure(productResult.Error!, productResult.ErrorType);
        }
        var product = productResult.Value!;
        if (cartItem.Quantity + amount > product.Stock)
        {
            return Result.Failure("Not enough Stock !!", ResultErrorType.Conflict);
        }
        var result = cartItem.IncreaseQuantity(amount);
        if (!result.IsSuccess)
        {
            return result;
        }
        _context.SaveChanges();
        return Result.Success();
    }
    public Result DecreaseQuantity(int userId, int productId, int amount)
    {
        if (amount <= 0)
        {
            return Result.Failure("Amount must be more than 0 !!", ResultErrorType.BadRequest);
        }
        var cartResult = GetCart(userId);
        if (!cartResult.IsSuccess)
        {
            return Result.Failure(cartResult.Error!, cartResult.ErrorType);
        }
        var cart = cartResult.Value!;
        var cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem == null)
        {
            return Result.Failure("Product is not in the cart !!", ResultErrorType.NotFound);
        }
        var result = cartItem.DecreaseQuantity(amount);
        if (!result.IsSuccess)
        {
            return result;
        }
        _context.SaveChanges();
        return Result.Success();
    }
    public Result ClearCart(int userId)
    {
        var cartResult = GetCart(userId);
        if (!cartResult.IsSuccess)
        {
            return Result.Failure(cartResult.Error!, cartResult.ErrorType);
        }
        var result = cartResult.Value!.Clear();
        if (!result.IsSuccess)
        {
            return result;
        }
        _context.SaveChanges();
        return Result.Success();
    }
}