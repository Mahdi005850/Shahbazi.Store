using Shahbazi.Store.DTOs;
using Shahbazi.Store.ResultPattern;
using Shahbazi.Store.Services;

namespace Shahbazi.Store.EndPoints;

internal static class UserEndPoints
{
    public static void MapUserEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", (UserServices userServices, UserCreatDto dto) =>
        {
            var result = userServices.AddUser(dto.FullName, dto.PhoneNumber, dto.Address);
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok("User Created Successfully !!");
        });
        app.MapGet("/api/users", (UserServices userServices) =>
        {
            var usersResult = userServices.GetAllUsers();
            var result = usersResult.Value!.Select(User => new UserGetAllDto
            {
                Id = User.Id,
                FullName = User.FullName,
                PhoneNumber = User.PhoneNumber,
                Address = User.Address
            });
            return Results.Ok(result);
        });
        app.MapGet("/api/users/{id}", (int id, UserServices userServices) =>
        {
            var userResult = userServices.GetUser(id);
            if (!userResult.IsSuccess)
            {
                return Results.NotFound(userResult.Error);
            }
            var user = userResult.Value!;
            var result = new UserGetDto
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };
            return Results.Ok(result);
        });
        app.MapPut("/api/users/{id}", (int id, UserUpdateDto dto, UserServices userServices) =>
        {
            var result = userServices.UpdateUser(id, dto.FullName, dto.PhoneNumber, dto.Address);
            if (!result.IsSuccess)
            {
                if (result.ErrorType == ResultErrorType.NotFound)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.BadRequest(result.Error);
            }
            return Results.Ok("User Updated Successfully !!");
        });
        app.MapDelete("/api/users/{id}", (int id, UserServices userServices) =>
        {
            var result = userServices.DeleteUser(id);
            if (!result.IsSuccess)
            {
                return Results.NotFound(result.Error);
            }
            var deleteResult = new UserDeleteDto
            {
                Id = id
            };
            return Results.Ok(deleteResult);
        });
    }
}