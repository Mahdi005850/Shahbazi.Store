using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.CodeResponse.ApiResults;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts");

        group.MapPost("", (CartCreateRequest request, CartServices service) => ToHttpResult(service.CreatCart(request.UserId), new(Success: "Cart created successfully.", BadRequest: "User id is invalid.", NotFound: "User not found.", Conflict: "This user already has a cart.")));

        group.MapGet("", (CartServices service) => ToHttpResult(service.GetAllCarts(), new(Success: "Carts retrieved successfully.")));

        group.MapGet("/{userId:int}", (int userId, CartServices service) =>
            ToHttpResult(service.GetCart(userId), new(Success: "Cart retrieved successfully.", BadRequest: "User id is invalid.", NotFound: "Cart not found.")));

        group.MapDelete("/{userId:int}/items", (int userId, CartServices service) => ToHttpResult(service.ClearCart(userId), new(Success: "Cart cleared successfully.", BadRequest: "User id is invalid.", NotFound: "Cart not found.")));

        group.MapPost("/{userId:int}/items", (int userId, CartItemCreateRequest request, CartServices service) => ToHttpResult(service.AddToCart(userId, request.ProductId, request.Quantity), new(Success: "Item added to cart successfully.", Conflict: "Not enough stock for this product.")));

        group.MapDelete("/{userId:int}/items/{productId:int}", (int userId, int productId, CartServices service) => ToHttpResult(service.RemoveFromCart(userId, productId), new(Success: "Item removed from cart successfully.")));

        group.MapPatch("/{userId:int}/items/{productId:int}/increase", (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(service.IncreaseQuantity(userId, productId, request.Amount), new(Success: "Item quantity increased successfully.", Conflict: "Not enough stock for this product.")));

        group.MapPatch("/{userId:int}/items/{productId:int}/decrease", (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(service.DecreaseQuantity(userId, productId, request.Amount), new(Success: "Item quantity decreased successfully.")));

        return app;
    }
}