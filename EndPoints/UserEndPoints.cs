using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapPost("", (UserCreateRequest request, UserServices service) => ToHttpResult(service.AddUser(request.FullName, request.PhoneNumber, request.Address)));

        group.MapGet("", (UserServices service) => ToHttpResult(service.GetAllUsers()));

        group.MapGet("/{id:int}", (int id, UserServices service) => ToHttpResult(service.GetUser(id)));

        group.MapPut("/{id:int}", (int id, UserUpdateRequest request, UserServices service) => ToHttpResult(service.UpdateUser(id, request.FullName, request.PhoneNumber, request.Address)));

        group.MapDelete("/{id:int}", (int id, UserServices service) => ToHttpResult(service.DeleteUser(id)));

        return app;
    }

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        return result.ErrorType switch
        {
            ResultErrorType.BadRequest => TypedResults.BadRequest(new { error = result.Error }),
            ResultErrorType.NotFound => TypedResults.NotFound(new { error = result.Error }),
            ResultErrorType.Conflict => TypedResults.Conflict(new { error = result.Error }),
            _ => TypedResults.Problem(result.Error)
        };
    }

    private static IResult ToHttpResult(Result result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(new { message = "Operation completed successfully" });
        }

        return result.ErrorType switch
        {
            ResultErrorType.BadRequest => TypedResults.BadRequest(new { error = result.Error }),
            ResultErrorType.NotFound => TypedResults.NotFound(new { error = result.Error }),
            ResultErrorType.Conflict => TypedResults.Conflict(new { error = result.Error }),
            _ => TypedResults.Problem(result.Error)
        };
    }
}