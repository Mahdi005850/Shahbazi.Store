using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.CodeResponse.ApiResults;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapPost("/{userId:int}", async (int userId, OrderCreateRequest request, OrderServices service) => ToCreatedResult(await service.CreateOrderAsync(userId, request.ShippingAddress), o => $"/api/orders/{o.Id}", new(Success: "Order created successfully.", Conflict: "Not enough stock for one or more products.")));

        group.MapGet("/{id:int}", async (int id, OrderServices service) => ToHttpResult(await service.GetOrderAsync(id), new(Success: "Order retrieved successfully.", NotFound: "Order not found.")));

        group.MapGet("/users/{userId:int}", async (int userId, OrderServices service) => ToHttpResult(await service.GetUserOrderAsync(userId), new(Success: "User orders retrieved successfully.", BadRequest: "User id is invalid.")));

        group.MapPut("/{id:int}/cancel", async (int id, OrderServices service) => ToHttpResult(await service.CancelOrderAsync(id), new(Success: "Order cancelled successfully.", BadRequest: "Order id is invalid.", NotFound: "Order not found.", Conflict: "This order cannot be cancelled.")));

        group.MapPut("/{id:int}/status", async (int id, OrderStatus status, OrderServices service) => ToHttpResult(await service.ChangeOrderStatusAsync(id, status), new(Success: "Order status updated successfully.", NotFound: "Order not found.", Conflict: "This status change is not allowed for the order.")));

        group.MapPut("/{id:int}/address", async (int id, OrderUpdateRequest request, OrderServices service) => ToHttpResult(await service.UpdateOrderAddressAsync(id, request.ShippingAddress), new(Success: "Order address updated successfully.", NotFound: "Order not found.", Conflict: "The address of this order can no longer be changed.")));

        group.MapGet("", async ([AsParameters] OrderQuery query, OrderServices service) => ToHttpResult(await service.GetOrdersAsync(query), new(Success: "Orders retrieved successfully.")));

        return app;
    }
}