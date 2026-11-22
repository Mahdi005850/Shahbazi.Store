using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapPost("/{userId:int}", async (int userId, OrderCreateRequest request, OrderServices service) => ToCreatedResult(await service.CreateOrderAsync(userId, request.ShippingAddress), o => $"/api/orders/{o.Id}"));

        group.MapGet("/{id:int}", async (int id, OrderServices service) => ToHttpResult(await service.GetOrderAsync(id)));

        group.MapGet("/users/{userId:int}", async (int userId, OrderServices service) => ToHttpResult(await service.GetUserOrderAsync(userId)));

        group.MapPut("/{id:int}/cancel", async (int id, OrderServices service) => ToHttpResult(await service.CancelOrderAsync(id)));

        group.MapPut("/{id:int}/status", async (int id, OrderStatus status, OrderServices service) => ToHttpResult(await service.ChangeOrderStatusAsync(id, status)));

        group.MapPut("/{id:int}/address", async (int id, OrderUpdateRequest request, OrderServices service) => ToHttpResult(await service.UpdateOrderAddressAsync(id, request.ShippingAddress)));

        group.MapGet("", async ([AsParameters] OrderQuery query, OrderServices service) => ToHttpResult(await service.GetOrdersAsync(query)));

        return app;
    }
}