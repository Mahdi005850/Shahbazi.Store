using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.Services;

public class AuthServices
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly TokenService _tokenService;

    public AuthServices(AppDbContext context, IPasswordHasher<User> passwordHasher, TokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    // Everybody who registers is a Buyer. Only an Admin can promote a user to Seller/Admin.
    public async Task<Result<UserGetResponse>> RegisterAsync(RegisterRequest request)
    {
        var validation = request.Validate();

        if (!validation.IsValid)
        {
            return Result<UserGetResponse>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var phoneNumber = request.PhoneNumber.Trim();

        if (await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber))
        {
            return Result<UserGetResponse>.ConflictError("A user with this phone number already exists.");
        }

        var user = new User(request.FullName.Trim(), phoneNumber, request.Address.Trim());
        user.SetPasswordHash(_passwordHasher.HashPassword(user, request.Password));

        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Two requests with the same phone number at the same time (the unique index protects us).
            return Result<UserGetResponse>.ConflictError("A user with this phone number already exists.");
        }

        return Result<UserGetResponse>.Success(user.ToGetResponse(), "User registered successfully.");
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var validation = request.Validate();

        if (!validation.IsValid)
        {
            return Result<LoginResponse>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        const string invalidCredentials = "Phone number or password is incorrect.";

        var phoneNumber = request.PhoneNumber.Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        // Same message for "no such user" and "wrong password", so nobody can find out which phone numbers exist.
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return Result<LoginResponse>.UnauthorizedError(invalidCredentials);
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verification == PasswordVerificationResult.Failed)
        {
            return Result<LoginResponse>.UnauthorizedError(invalidCredentials);
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.SetPasswordHash(_passwordHasher.HashPassword(user, request.Password));
            await _context.SaveChangesAsync();
        }

        var (token, expiresAtUtc) = _tokenService.CreateToken(user);

        return Result<LoginResponse>.Success(new LoginResponse(token, expiresAtUtc, user.ToGetResponse()), "Login successful.");
    }
}