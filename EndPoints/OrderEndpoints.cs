using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Services;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapPost("/{userId:int}", (int userId, OrderCreateRequest request, OrderServices service) => ToHttpResult(service.CreateOrder(userId, request.ShippingAddress)));

        group.MapGet("/{id:int}", (int id, OrderServices service) => ToHttpResult(service.GetOrder(id)));

        group.MapGet("/users/{userId:int}", (int userId, OrderServices service) => ToHttpResult(service.GetUserOrder(userId)));

        group.MapPut("/{id:int}/cancel", (int id, OrderServices service) => ToHttpResult(service.CancelOrder(id)));

        group.MapPut("/{id:int}/status", (int id, OrderStatus status, OrderServices service) => ToHttpResult(service.ChangeOrderStatus(id, status)));

        return app;
    }

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

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