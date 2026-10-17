using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.CodeResponse.ApiResults;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapPost("/{userId:int}", (int userId, OrderCreateRequest request, OrderServices service) => ToHttpResult(service.CreateOrder(userId, request.ShippingAddress), new(Success: "Order created successfully.", Conflict: "Not enough stock for one or more products.")));

        group.MapGet("/{id:int}", (int id, OrderServices service) => ToHttpResult(service.GetOrder(id), new(Success: "Order retrieved successfully.", NotFound: "Order not found.")));

        group.MapGet("/users/{userId:int}", (int userId, OrderServices service) => ToHttpResult(service.GetUserOrder(userId), new(Success: "User orders retrieved successfully.", BadRequest: "User id is invalid.")));

        group.MapPut("/{id:int}/cancel", (int id, OrderServices service) => ToHttpResult(service.CancelOrder(id), new(Success: "Order cancelled successfully.")));

        group.MapPut("/{id:int}/status", (int id, OrderStatus status, OrderServices service) => ToHttpResult(service.ChangeOrderStatus(id, status), new(Success: "Order status updated successfully.")));

        return app;
    }
}