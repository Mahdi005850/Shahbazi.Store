using System.Globalization;

namespace Shahbazi.Store.Common.Auth;

public class OwnerOrAdminFilter : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        if (http.Request.RouteValues.TryGetValue("userId", out var value)
            && int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var routeUserId)
            && http.User.CanAccessUser(routeUserId))
        {
            return next(context);
        }

        return ValueTask.FromResult<object?>(TypedResults.Forbid());
    }
}