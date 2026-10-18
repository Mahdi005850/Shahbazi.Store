using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Models;
using Shahbazi.Store.DTOs.Requests;

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
            return cartResult.ToFailure<CartGetResponse>();
        }

        return Result<CartGetResponse>.Success(ToGetResponse(cartResult.Value!));
    }

    public Result<CartGetResponse> CreatCart(int userId)
    {
        if (userId <= 0)
        {
            return Result<CartGetResponse>.BadRequestError("Invalid UserId !!");
        }

        if (!_context.Users.Any(u => u.Id == userId))
        {
            return Result<CartGetResponse>.NotFoundError("User didn't find !!");
        }

        if (_context.Carts.Any(c => c.UserId == userId))
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
            return Result.BadRequestError("Quantity must be greater than 0 !!");
        }

        var cartResult = GetCartModel(userId);
        if (!cartResult.IsSuccess)
        {
            return cartResult;
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == productId);

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

        _context.SaveChanges();
        return Result.Success();
    }
    public Result RemoveFromCart(int userId, int productId)
    {
        var validation = new CartItemDeleteRequest(productId).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var cartResult = GetCartModel(userId);
        if (!cartResult.IsSuccess)
        {
            return cartResult;
        }

        var cart = cartResult.Value!;
        var item = cart.CartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item is null) return Result.NotFoundError("Product is not in the cart !!");

        var result = cart.RemoveItem(item);
        if (!result.IsSuccess) return result;

        _context.SaveChanges();
        return Result.Success();
    }
    public Result IncreaseQuantity(int userId, int productId, int amount)
    {
        if (amount <= 0)
        {
            return Result.BadRequestError("Amount must be greater than 0 !!");
        }

        var cartResult = GetCartModel(userId);

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

        var product = _context.Products.FirstOrDefault(p => p.Id == productId);

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

        _context.SaveChanges();
        return Result.Success();
    }

    public Result DecreaseQuantity(int userId, int productId, int amount)
    {
        if (amount <= 0)
        {
            return Result.BadRequestError("Amount must be greater than 0 !!");
        }

        var cartResult = GetCartModel(userId);

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

        _context.SaveChanges();
        return Result.Success();
    }
    public Result ClearCart(int userId)
    {
        var validation = new CartDeleteRequest(userId).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var cartResult = GetCartModel(userId);
        if (!cartResult.IsSuccess)
        {
            return cartResult;
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
        {
            return Result<Cart>.BadRequestError("UserId is invalid !!");
        }

        var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);

        if (cart is null)
        {
            return Result<Cart>.NotFoundError("Cart didn't find !!");
        }

        return Result<Cart>.Success(cart);
    }

    private static CartGetResponse ToGetResponse(Cart cart) => new(cart.Id, cart.UserId, cart.CreatedAt, cart.UpdatedAt, cart.TotalPrice, cart.CartItems.Select(ToCartItemResponse).ToList());

    private static CartGetAllResponse ToGetAllResponse(Cart cart) => new(cart.Id, cart.UserId, cart.CreatedAt, cart.UpdatedAt, cart.TotalPrice);

    private static CartItemGetResponse ToCartItemResponse(CartItem item) => new(item.Id, item.ProductId, item.Quantity, item.UnitPrice, item.TotalPrice);
}
