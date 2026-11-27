using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // Creating a user is now done by /api/auth/register
        var group = app.MapGroup("/api/users").RequireAuthorization();

        group.MapGet("", async ([AsParameters] UserQuery query, UserServices service) => ToHttpResult(await service.GetUsersAsync(query))).RequireAuthorization(Policies.AdminOnly);

        group.MapGet("/{userId:int}", async (int userId, UserServices service) => ToHttpResult(await service.GetUserAsync(userId))).AddEndpointFilter<OwnerOrAdminFilter>();

        group.MapPut("/{userId:int}", async (int userId, UserUpdateRequest request, UserServices service) => ToHttpResult(await service.UpdateUserAsync(userId, request.FullName, request.PhoneNumber, request.Address))).AddEndpointFilter<OwnerOrAdminFilter>();

        group.MapDelete("/{userId:int}", async (int userId, UserServices service) => ToHttpResult(await service.DeleteUserAsync(userId))).RequireAuthorization(Policies.AdminOnly);

        group.MapPut("/{userId:int}/role", async (int userId, ChangeUserRoleRequest request, UserServices service) => ToHttpResult(await service.ChangeRoleAsync(userId, request.Role))).RequireAuthorization(Policies.AdminOnly);

        return app;
    }
}