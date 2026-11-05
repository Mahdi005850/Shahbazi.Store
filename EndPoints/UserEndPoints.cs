using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.CodeResponse.ApiResults;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users"); group.MapPost("", async (UserCreateRequest request, UserServices service) => ToCreatedResult(await service.AddUserAsync(request.FullName, request.PhoneNumber, request.Address), u => $"/api/users/{u.Id}", new(Success: "User created successfully.")));

        group.MapGet("", async ([AsParameters] UserQuery query, UserServices service) => ToHttpResult(await service.GetUsersAsync(query), new(Success: "Users retrieved successfully.")));

        group.MapGet("/{id:int}", async (int id, UserServices service) => ToHttpResult(await service.GetUserAsync(id), new(Success: "User retrieved successfully.", BadRequest: "User id is invalid.", NotFound: "User not found.")));

        group.MapPut("/{id:int}", async (int id, UserUpdateRequest request, UserServices service) => ToHttpResult(await service.UpdateUserAsync(id, request.FullName, request.PhoneNumber, request.Address), new(Success: "User updated successfully.", NotFound: "User not found.")));

        group.MapDelete("/{id:int}", async (int id, UserServices service) => ToHttpResult(await service.DeleteUserAsync(id), new(Success: "User deleted successfully.", BadRequest: "User id is invalid.", NotFound: "User not found.")));

        return app;
    }
}