using System.Security.Claims;

namespace Shahbazi.Store.Common.Auth;

public static class UserClaimsExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst("sub")?.Value;
        return int.TryParse(value, out var id) ? id : 0;
    }

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(Roles.Admin);

    public static bool IsStaff(this ClaimsPrincipal user) => user.IsInRole(Roles.Seller) || user.IsInRole(Roles.Admin);

    public static bool CanAccessUser(this ClaimsPrincipal user, int userId)
    {
        return user.IsAdmin() || (userId > 0 && user.GetUserId() == userId);
    }
}