using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Services;
using System.Security.Claims;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class SellerOrderEndpoints
{
    public static IEndpointRouteBuilder MapSellerOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/seller/orders").RequireAuthorization(Policies.SellerOrAdmin).WithTags("Seller - Orders");

        group.MapGet("", async ([AsParameters] OrderQuery query, ClaimsPrincipal user, OrderServices service) => ToHttpResult(await service.GetOrdersAsync(query, user.IsAdmin() ? null : user.GetUserId())));

        var one = group.MapGroup("/{id:int}").AddEndpointFilter<SellerOrderFilter>();

        one.MapGet("", async (int id, OrderServices service) => ToHttpResult(await service.GetOrderAsync(id)));

        one.MapPut("/status", async (int id, OrderStatus status, OrderServices service) => ToHttpResult(await service.ChangeOrderStatusAsync(id, status)));

        return app;
    }
}