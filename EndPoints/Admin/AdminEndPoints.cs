using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var admin = app.MapGroup("/api/admin").RequireAuthorization(Policies.AdminOnly);

        var users = admin.MapGroup("/users").WithTags("Admin - Users");

        users.MapGet("", async ([AsParameters] UserQuery query, UserServices service) => ToHttpResult(await service.GetUsersAsync(query)));

        users.MapGet("/{userId:int}", async (int userId, UserServices service) => ToHttpResult(await service.GetUserAsync(userId)));

        users.MapDelete("/{userId:int}", async (int userId, UserServices service) => ToHttpResult(await service.DeleteUserAsync(userId)));

        users.MapPut("/{userId:int}/role", async (int userId, ChangeUserRoleRequest request, UserServices service) => ToHttpResult(await service.ChangeRoleAsync(userId, request.Role)));

        admin.MapGet("/carts", async (CartServices service) => ToHttpResult(await service.GetAllCartsAsync())).WithTags("Admin - Carts");

        return app;
    }
}