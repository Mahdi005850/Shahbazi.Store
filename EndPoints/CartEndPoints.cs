using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.CodeResponse.ApiResults;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts");

        group.MapPost("", async (CartCreateRequest request, CartServices service) => ToCreatedResult(await service.CreateCartAsync(request.UserId), _ => $"/api/carts/{request.UserId}", new(Success: "Cart created successfully.", BadRequest: "User id is invalid.", NotFound: "User not found.", Conflict: "This user already has a cart.")));

        group.MapGet("", async (CartServices service) => ToHttpResult(await service.GetAllCartsAsync(), new(Success: "Carts retrieved successfully.")));

        group.MapGet("/{userId:int}", async (int userId, CartServices service) => ToHttpResult(await service.GetCartAsync(userId), new(Success: "Cart retrieved successfully.", BadRequest: "User id is invalid.", NotFound: "Cart not found.")));

        group.MapDelete("/{userId:int}/items", async (int userId, CartServices service) => ToHttpResult(await service.ClearCartAsync(userId), new(Success: "Cart cleared successfully.", BadRequest: "User id is invalid.", NotFound: "Cart not found.")));

        group.MapPost("/{userId:int}/items", async (int userId, CartItemCreateRequest request, CartServices service) => ToHttpResult(await service.AddToCartAsync(userId, request.ProductId, request.Quantity), new(Success: "Item added to cart successfully.", Conflict: "Not enough stock for this product.")));

        group.MapDelete("/{userId:int}/items/{productId:int}", async (int userId, int productId, CartServices service) => ToHttpResult(await service.RemoveFromCartAsync(userId, productId), new(Success: "Item removed from cart successfully.")));

        group.MapPatch("/{userId:int}/items/{productId:int}/increase", async (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(await service.IncreaseQuantityAsync(userId, productId, request.Amount), new(Success: "Item quantity increased successfully.", Conflict: "Not enough stock for this product.")));

        group.MapPatch("/{userId:int}/items/{productId:int}/decrease", async (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(await service.DecreaseQuantityAsync(userId, productId, request.Amount), new(Success: "Item quantity decreased successfully.")));

        return app;
    }
}
