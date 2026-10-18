using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.CodeResponse.ApiResults;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapPost("", (UserCreateRequest request, UserServices service) => ToCreatedResult(service.AddUser(request.FullName, request.PhoneNumber, request.Address), u => $"/api/users/{u.Id}", new(Success: "User created successfully.")));

        group.MapGet("", ([AsParameters] UserQuery query, UserServices service) => ToHttpResult(service.GetUsers(query), new(Success: "Users retrieved successfully.")));

        group.MapGet("/{id:int}", (int id, UserServices service) => ToHttpResult(service.GetUser(id), new(Success: "User retrieved successfully.", BadRequest: "User id is invalid.", NotFound: "User not found.")));

        group.MapPut("/{id:int}", (int id, UserUpdateRequest request, UserServices service) => ToHttpResult(service.UpdateUser(id, request.FullName, request.PhoneNumber, request.Address), new(Success: "User updated successfully.", NotFound: "User not found.")));

        group.MapDelete("/{id:int}", (int id, UserServices service) => ToHttpResult(service.DeleteUser(id), new(Success: "User deleted successfully.", BadRequest: "User id is invalid.", NotFound: "User not found.")));

        return app;
    }
}