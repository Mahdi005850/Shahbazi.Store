using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Services;
using System.Security.Claims;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").RequireAuthorization();

        group.MapPost("/{userId:int}", async (int userId, OrderCreateRequest request, OrderServices service) => ToCreatedResult(await service.CreateOrderAsync(userId, request.ShippingAddress), o => $"/api/orders/{o.Id}")).AddEndpointFilter<OwnerOrAdminFilter>();

        group.MapGet("/users/{userId:int}", async (int userId, OrderServices service) => ToHttpResult(await service.GetUserOrderAsync(userId))).AddEndpointFilter<OwnerOrAdminFilter>();

        group.MapGet("/{id:int}", async (int id, ClaimsPrincipal user, OrderServices service) =>
        {
            var result = await service.GetOrderAsync(id);

            if (result.IsSuccess && !user.IsStaff() && result.Value!.UserId != user.GetUserId())
            {
                return Results.Forbid();
            }

            return ToHttpResult(result);
        });

        group.MapPut("/{id:int}/cancel", async (int id, ClaimsPrincipal user, OrderServices service) =>
        {
            if (!await CanAccessOrderAsync(id, user, service))
            {
                return Results.Forbid();
            }

            return ToHttpResult(await service.CancelOrderAsync(id));
        });

        group.MapPut("/{id:int}/address", async (int id, OrderUpdateRequest request, ClaimsPrincipal user, OrderServices service) =>
        {
            if (!await CanAccessOrderAsync(id, user, service))
            {
                return Results.Forbid();
            }

            return ToHttpResult(await service.UpdateOrderAddressAsync(id, request.ShippingAddress));
        });

        group.MapGet("", async ([AsParameters] OrderQuery query, OrderServices service) => ToHttpResult(await service.GetOrdersAsync(query))).RequireAuthorization(Policies.SellerOrAdmin);

        group.MapPut("/{id:int}/status", async (int id, OrderStatus status, OrderServices service) => ToHttpResult(await service.ChangeOrderStatusAsync(id, status))).RequireAuthorization(Policies.SellerOrAdmin);

        return app;
    }

    private static async Task<bool> CanAccessOrderAsync(int orderId, ClaimsPrincipal user, OrderServices service)
    {
        if (user.IsStaff())
        {
            return true;
        }

        var order = await service.GetOrderAsync(orderId);

        return !order.IsSuccess || order.Value!.UserId == user.GetUserId();
    }
}