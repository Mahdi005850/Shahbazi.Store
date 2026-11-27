using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using System.Security.Claims;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class BuyerCartEndpoints
{
    public static IEndpointRouteBuilder MapBuyerCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/buyer/cart").RequireAuthorization(Policies.BuyerOnly).WithTags("Buyer - Cart");

        group.MapPost("", async (ClaimsPrincipal user, CartServices service) => ToCreatedResult(await service.CreateCartAsync(user.GetUserId()), _ => "/api/buyer/cart"));

        group.MapGet("", async (ClaimsPrincipal user, CartServices service) => ToHttpResult(await service.GetCartAsync(user.GetUserId())));

        group.MapDelete("/items", async (ClaimsPrincipal user, CartServices service) => ToHttpResult(await service.ClearCartAsync(user.GetUserId())));

        group.MapPost("/items", async (CartItemCreateRequest request, ClaimsPrincipal user, CartServices service) => ToHttpResult(await service.AddToCartAsync(user.GetUserId(), request.ProductId, request.Quantity)));

        group.MapDelete("/items/{productId:int}", async (int productId, ClaimsPrincipal user, CartServices service) => ToHttpResult(await service.RemoveFromCartAsync(user.GetUserId(), productId)));

        group.MapPatch("/items/{productId:int}/increase", async (int productId, CartItemQuantityRequest request, ClaimsPrincipal user, CartServices service) => ToHttpResult(await service.IncreaseQuantityAsync(user.GetUserId(), productId, request.Amount)));

        group.MapPatch("/items/{productId:int}/decrease", async (int productId, CartItemQuantityRequest request, ClaimsPrincipal user, CartServices service) => ToHttpResult(await service.DecreaseQuantityAsync(user.GetUserId(), productId, request.Amount)));

        return app;
    }
}