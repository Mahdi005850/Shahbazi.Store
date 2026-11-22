using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts");

        group.MapPost("", async (CartCreateRequest request, CartServices service) => ToCreatedResult(await service.CreateCartAsync(request.UserId), _ => $"/api/carts/{request.UserId}"));

        group.MapGet("", async (CartServices service) => ToHttpResult(await service.GetAllCartsAsync()));

        group.MapGet("/{userId:int}", async (int userId, CartServices service) => ToHttpResult(await service.GetCartAsync(userId)));

        group.MapDelete("/{userId:int}/items", async (int userId, CartServices service) => ToHttpResult(await service.ClearCartAsync(userId)));

        group.MapPost("/{userId:int}/items", async (int userId, CartItemCreateRequest request, CartServices service) => ToHttpResult(await service.AddToCartAsync(userId, request.ProductId, request.Quantity)));

        group.MapDelete("/{userId:int}/items/{productId:int}", async (int userId, int productId, CartServices service) => ToHttpResult(await service.RemoveFromCartAsync(userId, productId)));

        group.MapPatch("/{userId:int}/items/{productId:int}/increase", async (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(await service.IncreaseQuantityAsync(userId, productId, request.Amount)));

        group.MapPatch("/{userId:int}/items/{productId:int}/decrease", async (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(await service.DecreaseQuantityAsync(userId, productId, request.Amount)));

        return app;
    }
}
