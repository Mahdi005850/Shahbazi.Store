using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using System.Security.Claims;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterRequest request, AuthServices service) => ToCreatedResult(await service.RegisterAsync(request), u => $"/api/users/{u.Id}")).AllowAnonymous();

        group.MapPost("/login", async (LoginRequest request, AuthServices service) => ToHttpResult(await service.LoginAsync(request))).AllowAnonymous();

        group.MapGet("/me", async (ClaimsPrincipal user, UserServices service) => ToHttpResult(await service.GetUserAsync(user.GetUserId()))).RequireAuthorization();

        group.MapPut("/me", async (UserUpdateRequest request, ClaimsPrincipal user, UserServices service) => ToHttpResult(await service.UpdateUserAsync(user.GetUserId(), request.FullName, request.PhoneNumber, request.Address))).RequireAuthorization();

        return app;
    }
}