using Shahbazi.Store.Services;
using System.Globalization;

namespace Shahbazi.Store.Common.Auth;

internal static class RouteValueExtensions
{
    public static bool TryGetRouteInt(this HttpContext http, string name, out int value)
    {
        value = 0;
        return http.Request.RouteValues.TryGetValue(name, out var raw)
            && int.TryParse(Convert.ToString(raw, CultureInfo.InvariantCulture), out value);
    }
}

public class ProductOwnerOrAdminFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        if (!http.TryGetRouteInt("id", out var productId))
            return TypedResults.Forbid();

        if (http.User.IsAdmin())
            return await next(context);

        var sellerId = await http.RequestServices.GetRequiredService<ProductServices>().GetSellerIdAsync(productId);

        if (sellerId is null)
            return await next(context);

        return sellerId == http.User.GetUserId() ? await next(context) : TypedResults.Forbid();
    }
}

public class OrderOwnerFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        if (!http.TryGetRouteInt("id", out var orderId))
            return TypedResults.Forbid();

        var ownerId = await http.RequestServices.GetRequiredService<OrderServices>().GetOrderOwnerIdAsync(orderId);

        if (ownerId is null)
            return await next(context);

        return ownerId == http.User.GetUserId() ? await next(context) : TypedResults.Forbid();
    }
}

public class SellerOrderFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        if (!http.TryGetRouteInt("id", out var orderId))
            return TypedResults.Forbid();

        if (http.User.IsAdmin())
            return await next(context);

        var hasItem = await http.RequestServices.GetRequiredService<OrderServices>()
            .SellerHasItemInOrderAsync(orderId, http.User.GetUserId());

        return hasItem ? await next(context) : TypedResults.Forbid();
    }
}