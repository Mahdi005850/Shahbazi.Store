using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;

namespace Shahbazi.Store.EndPoints;

internal static class CartEndPoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/carts/{userId}", (int userId, CartServices cartServices) =>
        {
            var result = cartServices.CreatCart(userId);
            if (!result.IsSuccess)
            {
                if (result.ErrorType == ResultErrorType.Conflict)
                {
                    return Results.Conflict(result.Error);
                }
                return Results.BadRequest(result.Error);
            }
            return Results.Ok(result.Value);
        });
        app.MapGet("/api/carts/{userId}", (int userId, CartServices cartServices) =>
        {
            var result = cartServices.GetCart(userId);
            if (!result.IsSuccess)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok(result.Value);
        });
        app.MapPost("/api/carts/{userId}/items", (int userId, CartItemCreateDto dto, CartServices cartServices) =>
            {
                var result = cartServices.AddToCart(userId, dto.ProductId, dto.Quantity);
                if (!result.IsSuccess)
                {
                    if (result.ErrorType == ResultErrorType.NotFound)
                    {
                        return Results.NotFound(result.Error);
                    }
                    if (result.ErrorType == ResultErrorType.Conflict)
                    {
                        return Results.Conflict(result.Error);
                    }
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok("Product Added To Cart Successfully !!");
            });
        app.MapPut("/api/carts/{userId}/items/{productId}/increase", (int userId, int productId, CartItemQuantityDto dto, CartServices cartServices) =>
            {
                var result = cartServices.IncreaseQuantity(userId, productId, dto.Amount);
                if (!result.IsSuccess)
                {
                    if (result.ErrorType == ResultErrorType.NotFound)
                    {
                        return Results.NotFound(result.Error);
                    }
                    if (result.ErrorType == ResultErrorType.Conflict)
                    {
                        return Results.Conflict(result.Error);
                    }
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok("Cart Item Quantity Increased Successfully !!");
            });
        app.MapPut("/api/carts/{userId}/items/{productId}/decrease", (int userId, int productId, CartItemQuantityDto dto, CartServices cartServices) =>
            {
                var result = cartServices.DecreaseQuantity(userId, productId, dto.Amount);
                if (!result.IsSuccess)
                {
                    if (result.ErrorType == ResultErrorType.NotFound)
                    {
                        return Results.NotFound(result.Error);
                    }
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok("Cart Item Quantity Decreased Successfully !!");
            });
        app.MapDelete("/api/carts/{userId}/items", (int userId, CartServices cartServices) =>
            {
                var result = cartServices.ClearCart(userId);
                if (!result.IsSuccess)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok("Cart Cleared Successfully !!");
            });
        app.MapDelete("/api/carts/{userId}/items/{productId}", (int userId, int productId, CartServices cartServices) =>
        {
            var result = cartServices.RemoveFromCart(userId, productId);
            if (!result.IsSuccess)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok("Product Removed From Cart Successfully !!");
        });
    }
}