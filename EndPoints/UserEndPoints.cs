using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users"); group.MapPost("", async (UserCreateRequest request, UserServices service) => ToCreatedResult(await service.AddUserAsync(request.FullName, request.PhoneNumber, request.Address), u => $"/api/users/{u.Id}"));

        group.MapGet("", async ([AsParameters] UserQuery query, UserServices service) => ToHttpResult(await service.GetUsersAsync(query)));

        group.MapGet("/{id:int}", async (int id, UserServices service) => ToHttpResult(await service.GetUserAsync(id)));

        group.MapPut("/{id:int}", async (int id, UserUpdateRequest request, UserServices service) => ToHttpResult(await service.UpdateUserAsync(id, request.FullName, request.PhoneNumber, request.Address)));

        group.MapDelete("/{id:int}", async (int id, UserServices service) => ToHttpResult(await service.DeleteUserAsync(id)));

        return app;
    }
}