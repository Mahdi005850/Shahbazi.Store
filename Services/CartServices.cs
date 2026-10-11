using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.Services;

public class CartServices
{
    private readonly AppDbContext _context;

    public CartServices(ProductServices productServices, AppDbContext context)
    {
        _context = context;
    }

    public Result<CartGetResponse> GetCart(int userId)
    {
        var cartResult = GetCartModel(userId);
        if (!cartResult.IsSuccess)
        {
            return Result<CartGetResponse>.Failure(cartResult.Error!, cartResult.ErrorType);
        }

        return Result<CartGetResponse>.Success(ToGetResponse(cartResult.Value!));
    }

    public Result<CartGetResponse> CreatCart(int userId)
    {
        if (userId <= 0)
        {
            return Result<CartGetResponse>.Failure("Invalid UserId !!", ResultErrorType.BadRequest);
        }

        if (!_context.Users.Any(u => u.Id == userId))
        {
            return Result<CartGetResponse>.Failure("User didn't find !!", ResultErrorType.NotFound);
        }
        if (_context.Carts.Any(c => c.UserId == userId))
            return Result<CartGetResponse>.Failure("This User has an existing Cart !!", ResultErrorType.Conflict);

        var cartResult = Cart.Create(userId);

        if (!cartResult.IsSuccess)
        {
            return Result<CartGetResponse>.Failure(cartResult.Error!, cartResult.ErrorType);
        }

        var cart = cartResult.Value!;

        _context.Carts.Add(cart);
        _context.SaveChanges();

        return Result<CartGetResponse>.Success(ToGetResponse(cart));
    }

    public Result<List<CartGetAllResponse>> GetAllCarts()
    {
        var carts = _context.Carts.Include(c => c.CartItems).AsNoTracking().ToList();

        return Result<List<CartGetAllResponse>>.Success(carts.Select(ToGetAllResponse).ToList());
    }

    public Result AddToCart(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure("Quantity must be greater than 0 !!", ResultErrorType.BadRequest);
        }

        var cartResult = GetCartModel(userId);
        if (!cartResult.IsSuccess)
        {
            return Result.Failure(cartResult.Error!, cartResult.ErrorType);
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == productId);

        if (product is null)
        {
            return Result.Failure("Product didn't find !!", ResultErrorType.NotFound);
        }

        var cart = cartResult.Value!;

        var existingItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

        var requestedTotal = quantity + (existingItem?.Quantity ?? 0);

        if (requestedTotal > product.Stock)
        {
            return Result.Failure("Not enough stock !!", ResultErrorType.Conflict);
        }

        if (existingItem is not null)
        {
            var increaseResult = existingItem.IncreaseQuantity(quantity);

            if (!increaseResult.IsSuccess) return increaseResult;
        }

        else
        {
            var itemResult = CartItem.Create(cart.Id, product.Id, quantity, product.Price);

            if (!itemResult.IsSuccess)
            {
                return Result.Failure(itemResult.Error!, itemResult.ErrorType);
            }

            var addResult = cart.AddItem(itemResult.Value!);
            if (!addResult.IsSuccess) return addResult;
        }

        _context.SaveChanges();
        return Result.Success();
    }

    public Result RemoveFromCart(int userId, int productId)
    {
        var cartResult = GetCartModel(userId);
        if (!cartResult.IsSuccess)
        {
            return Result.Failure(cartResult.Error!, cartResult.ErrorType);
        }

        var cart = cartResult.Value!;
        var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item is null) return Result.Failure("Product is not in the cart !!", ResultErrorType.NotFound);

        var result = cart.RemoveItem(item);
        if (!result.IsSuccess) return result;

        _context.SaveChanges();
        return Result.Success();
    }

    public Result IncreaseQuantity(int userId, int productId, int amount)
    {
        if (amount <= 0)
        {
            return Result.Failure("Amount must be greater than 0 !!", ResultErrorType.BadRequest);
        }

        var cartResult = GetCartModel(userId);

        if (!cartResult.IsSuccess)
        {
            return Result.Failure(cartResult.Error!, cartResult.ErrorType);
        }

        var cart = cartResult.Value!;

        var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
        {
            return Result.Failure("Product is not in the cart !!", ResultErrorType.NotFound);
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == productId);

        if (product is null)
        {
            return Result.Failure("Product didn't find !!", ResultErrorType.NotFound);
        }

        if (item.Quantity + amount > product.Stock)
        {
            return Result.Failure("Not enough stock !!", ResultErrorType.Conflict);
        }

        var result = item.IncreaseQuantity(amount);

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
            return Result.Failure("Amount must be greater than 0 !!", ResultErrorType.BadRequest);
        }

        var cartResult = GetCartModel(userId);

        if (!cartResult.IsSuccess)
        {
            return Result.Failure(cartResult.Error!, cartResult.ErrorType);
        }

        var item = cartResult.Value!.CartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
        {
            return Result.Failure("Product is not in the cart !!", ResultErrorType.NotFound);
        }

        var result = item.DecreaseQuantity(amount);

        if (!result.IsSuccess)
        {
            return result;
        }

        _context.SaveChanges();
        return Result.Success();
    }

    public Result ClearCart(int userId)
    {
        var cartResult = GetCartModel(userId);
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

    private Result<Cart> GetCartModel(int userId)
    {
        if (userId <= 0)
            return Result<Cart>.Failure("UserId is invalid !!", ResultErrorType.BadRequest);

        var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);
        if (cart is null)
            return Result<Cart>.Failure("Cart didn't find !!", ResultErrorType.NotFound);

        return Result<Cart>.Success(cart);
    }

    private static CartGetResponse ToGetResponse(Cart cart) => new(cart.Id, cart.UserId, cart.CreatedAt, cart.UpdatedAt, cart.TotalPrice, cart.CartItems.Select(ToCartItemResponse).ToList());

    private static CartGetAllResponse ToGetAllResponse(Cart cart) => new(cart.Id, cart.UserId, cart.CreatedAt, cart.UpdatedAt, cart.TotalPrice);

    private static CartItemGetResponse ToCartItemResponse(CartItem item) => new(item.Id, item.ProductId, item.Quantity, item.UnitPrice, item.TotalPrice);
}