using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using System.Security.Claims;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class BuyerOrderEndpoints
{
    public static IEndpointRouteBuilder MapBuyerOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/buyer/orders").RequireAuthorization(Policies.BuyerOnly).WithTags("Buyer - Orders");

        group.MapPost("", async (OrderCreateRequest request, ClaimsPrincipal user, OrderServices service) => ToCreatedResult(await service.CreateOrderAsync(user.GetUserId(), request.ShippingAddress), o => $"/api/buyer/orders/{o.Id}"));

        group.MapGet("", async (ClaimsPrincipal user, OrderServices service) => ToHttpResult(await service.GetUserOrderAsync(user.GetUserId())));

        var own = group.MapGroup("/{id:int}").AddEndpointFilter<OrderOwnerFilter>();

        own.MapGet("", async (int id, OrderServices service) => ToHttpResult(await service.GetOrderAsync(id)));

        own.MapPut("/cancel", async (int id, OrderServices service) => ToHttpResult(await service.CancelOrderAsync(id)));

        own.MapPut("/address", async (int id, OrderUpdateRequest request, OrderServices service) => ToHttpResult(await service.UpdateOrderAddressAsync(id, request.ShippingAddress)));

        return app;
    }
}