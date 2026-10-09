using Shahbazi.Store.Common.Enums;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;

namespace Shahbazi.Store.EndPoints;

internal static class OrderEndpoints
{
    public static void MapOrdersEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/orders/{userId}", (int userId, OrderCreateDto dto, OrderServices orderServices) =>
            {
                var result = orderServices.CreateOrder(userId, dto.ShippingAddress);
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
                return Results.Ok("Order Created Successfully !!");
            });
        app.MapGet("/api/orders/{id}", (int id, OrderServices orderServices) =>
            {
                var result = orderServices.GetOrder(id);
                if (!result.IsSuccess)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            });
        app.MapGet("/api/users/{userId}/orders", (int userId, OrderServices orderServices) =>
            {
                var result = orderServices.GetUserOrder(userId);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            });
        app.MapPut("/api/orders/{id}/cancel", (int id, OrderServices orderServices) =>
            {
                var result = orderServices.CancelOrder(id);
                if (!result.IsSuccess)
                {
                    if (result.ErrorType == ResultErrorType.NotFound)
                    {
                        return Results.NotFound(result.Error);
                    }
                    return Results.Conflict(result.Error);
                }
                return Results.Ok("Order Cancelled Successfully !!");
            });
        app.MapPut("/api/orders/{id}/status",
            (int id, OrderStatus status, OrderServices orderServices) =>
            {
                var result = orderServices.ChangeOrderStatus(id, status);
                if (!result.IsSuccess)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok("Order Status Changed Successfully !!");
            });
    }
}