using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using System.Security.Claims;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts").RequireAuthorization();

        // A user can only create a cart for himself (UserId comes in the body, so we check it here)
        group.MapPost("", async (CartCreateRequest request, ClaimsPrincipal user, CartServices service) =>
            !user.CanAccessUser(request.UserId)
                ? Results.Forbid()
                : ToCreatedResult(await service.CreateCartAsync(request.UserId), _ => $"/api/carts/{request.UserId}"));

        // All carts: Admin only
        group.MapGet("", async (CartServices service) => ToHttpResult(await service.GetAllCartsAsync())).RequireAuthorization(Policies.AdminOnly);

        // Everything under /api/carts/{userId} is allowed only for the owner (or Admin)
        var own = group.MapGroup("/{userId:int}").AddEndpointFilter<OwnerOrAdminFilter>();

        own.MapGet("", async (int userId, CartServices service) => ToHttpResult(await service.GetCartAsync(userId)));

        own.MapDelete("/items", async (int userId, CartServices service) => ToHttpResult(await service.ClearCartAsync(userId)));

        own.MapPost("/items", async (int userId, CartItemCreateRequest request, CartServices service) => ToHttpResult(await service.AddToCartAsync(userId, request.ProductId, request.Quantity)));

        own.MapDelete("/items/{productId:int}", async (int userId, int productId, CartServices service) => ToHttpResult(await service.RemoveFromCartAsync(userId, productId)));

        own.MapPatch("/items/{productId:int}/increase", async (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(await service.IncreaseQuantityAsync(userId, productId, request.Amount)));

        own.MapPatch("/items/{productId:int}/decrease", async (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(await service.DecreaseQuantityAsync(userId, productId, request.Amount)));

        return app;
    }
}