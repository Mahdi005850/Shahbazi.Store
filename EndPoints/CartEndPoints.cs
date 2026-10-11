using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts");

        group.MapPost("", (CartCreateRequest request, CartServices service) => ToHttpResult(service.CreatCart(request.UserId)));

        group.MapGet("", (CartServices service) => ToHttpResult(service.GetAllCarts()));

        group.MapGet("/{userId:int}", (int userId, CartServices service) => ToHttpResult(service.GetCart(userId)));

        group.MapDelete("/{userId:int}/items", (int userId, CartServices service) => ToHttpResult(service.ClearCart(userId)));

        group.MapPost("/{userId:int}/items", (int userId, CartItemCreateRequest request, CartServices service) => ToHttpResult(service.AddToCart(userId, request.ProductId, request.Quantity)));

        group.MapDelete("/{userId:int}/items/{productId:int}", (int userId, int productId, CartServices service) => ToHttpResult(service.RemoveFromCart(userId, productId)));

        group.MapPatch("/{userId:int}/items/{productId:int}/increase", (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(service.IncreaseQuantity(userId, productId, request.Amount)));

        group.MapPatch("/{userId:int}/items/{productId:int}/decrease", (int userId, int productId, CartItemQuantityRequest request, CartServices service) => ToHttpResult(service.DecreaseQuantity(userId, productId, request.Amount)));

        return app;
    }

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        return result.ErrorType switch
        {
            ResultErrorType.BadRequest => TypedResults.BadRequest(new { error = result.Error }),
            ResultErrorType.NotFound => TypedResults.NotFound(new { error = result.Error }),
            ResultErrorType.Conflict => TypedResults.Conflict(new { error = result.Error }),
            _ => TypedResults.Problem(result.Error)
        };
    }

    private static IResult ToHttpResult(Result result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(new { message = "Operation completed successfully" });
        }

        return result.ErrorType switch
        {
            ResultErrorType.BadRequest => TypedResults.BadRequest(new { error = result.Error }),
            ResultErrorType.NotFound => TypedResults.NotFound(new { error = result.Error }),
            ResultErrorType.Conflict => TypedResults.Conflict(new { error = result.Error }),
            _ => TypedResults.Problem(result.Error)
        };
    }
}