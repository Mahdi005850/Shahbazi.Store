using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Requests;
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

    public async Task<Result<CartGetResponse>> GetCartAsync(int userId)
    {
        var cartResult = await GetCartModelAsync(userId);

        if (!cartResult.IsSuccess)
        {
            return cartResult.ToFailure<CartGetResponse>();
        }

        return Result<CartGetResponse>.Success(cartResult.Value!.ToGetResponse());
    }

    public async Task<Result<CartGetResponse>> CreateCartAsync(int userId)
    {
        if (userId <= 0)
        {
            return Result<CartGetResponse>.BadRequestError("Invalid UserId !!");
        }

        if (!await _context.Users.AnyAsync(u => u.Id == userId))
        {
            return Result<CartGetResponse>.NotFoundError("User didn't find !!");
        }

        if (await _context.Carts.AnyAsync(c => c.UserId == userId))
        {
            return Result<CartGetResponse>.ConflictError("This User has an existing Cart !!");
        }

        var cartResult = Cart.Create(userId);

        if (!cartResult.IsSuccess)
        {
            return cartResult.ToFailure<CartGetResponse>();
        }

        var cart = cartResult.Value!;

        _context.Carts.Add(cart);

        await _context.SaveChangesAsync();

        return Result<CartGetResponse>.Success(cart.ToGetResponse());
    }

    public async Task<Result<List<CartGetAllResponse>>> GetAllCartsAsync()
    {
        var carts = await _context.Carts.Include(c => c.CartItems).AsNoTracking().ToListAsync();

        return Result<List<CartGetAllResponse>>.Success(carts.Select(c => c.ToGetAllResponse()).ToList());
    }

    public async Task<Result> AddToCartAsync(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            return Result.BadRequestError("Quantity must be greater than 0 !!");
        }

        var cartResult = await GetCartModelAsync(userId);

        if (!cartResult.IsSuccess)
        {
            return cartResult;
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
        {
            return Result.NotFoundError("Product didn't find !!");
        }

        var cart = cartResult.Value!;

        var existingItem = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

        var requestedTotal = quantity + (existingItem?.Quantity ?? 0);

        if (requestedTotal > product.Stock)
        {
            return Result.ConflictError("Not enough stock !!");
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
                return itemResult;
            }

            var addResult = cart.AddItem(itemResult.Value!);
            if (!addResult.IsSuccess) return addResult;
        }

        await _context.SaveChangesAsync();

        return Result.Success();
    }
    public async Task<Result> RemoveFromCartAsync(int userId, int productId)
    {
        var validation = new CartItemDeleteRequest(productId).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var cartResult = await GetCartModelAsync(userId);

        if (!cartResult.IsSuccess)
        {
            return cartResult;
        }

        var cart = cartResult.Value!;
        var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item is null) return Result.NotFoundError("Product is not in the cart !!");

        var result = cart.RemoveItem(item);
        if (!result.IsSuccess) return result;

        await _context.SaveChangesAsync();

        return Result.Success();
    }
    public async Task<Result> IncreaseQuantityAsync(int userId, int productId, int amount)
    {
        if (amount <= 0)
        {
            return Result.BadRequestError("Amount must be greater than 0 !!");
        }

        var cartResult = await GetCartModelAsync(userId);

        if (!cartResult.IsSuccess)
        {
            return cartResult;
        }

        var cart = cartResult.Value!;

        var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
        {
            return Result.NotFoundError("Product is not in the cart !!");
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
        {
            return Result.NotFoundError("Product didn't find !!");
        }

        if (item.Quantity + amount > product.Stock)
        {
            return Result.ConflictError("Not enough stock !!");
        }

        var result = item.IncreaseQuantity(amount);

        if (!result.IsSuccess)
        {
            return result;
        }

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DecreaseQuantityAsync(int userId, int productId, int amount)
    {
        if (amount <= 0)
        {
            return Result.BadRequestError("Amount must be greater than 0 !!");
        }

        var cartResult = await GetCartModelAsync(userId);

        if (!cartResult.IsSuccess)
        {
            return cartResult;
        }

        var item = cartResult.Value!.CartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
        {
            return Result.NotFoundError("Product is not in the cart !!");
        }

        var result = item.DecreaseQuantity(amount);

        if (!result.IsSuccess)
        {
            return result;
        }

        await _context.SaveChangesAsync();

        return Result.Success();
    }
    public async Task<Result> ClearCartAsync(int userId)
    {
        var validation = new CartDeleteRequest(userId).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var cartResult = await GetCartModelAsync(userId);

        if (!cartResult.IsSuccess)
        {
            return cartResult;
        }

        var result = cartResult.Value!.Clear();

        if (!result.IsSuccess)
        {
            return result;
        }

        await _context.SaveChangesAsync();

        return Result.Success();
    }
    private async Task<Result<Cart>> GetCartModelAsync(int userId)
    {
        if (userId <= 0)
        {
            return Result<Cart>.BadRequestError("UserId is invalid !!");
        }

        var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null)
        {
            return Result<Cart>.NotFoundError("Cart didn't find !!");
        }

        return Result<Cart>.Success(cart);
    }
}
