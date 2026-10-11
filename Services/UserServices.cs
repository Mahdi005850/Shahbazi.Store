using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.Services;

public class UserServices
{
    private readonly AppDbContext _context;

    public UserServices(AppDbContext context)
    {
        _context = context;
    }

    public Result<UserGetResponse> GetUser(int userId)
    {
        if (userId <= 0)
        {
            return Result<UserGetResponse>.Failure("User id is invalid", ResultErrorType.BadRequest);
        }

        var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);

        if (user is null)
        {
            return Result<UserGetResponse>.Failure("User didn't find !!", ResultErrorType.NotFound);
        }

        return Result<UserGetResponse>.Success(ToGetResponse(user));
    }

    public Result<UserGetResponse> AddUser(string fullName, string phoneNumber, string address)
    {
        var request = new UserCreateRequest(fullName, phoneNumber, address);
        var validation = request.Validate();

        if (!validation.IsValid)
        {
            return Result<UserGetResponse>.Failure(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)), ResultErrorType.BadRequest);
        }

        var user = new User(fullName, phoneNumber, address);

        _context.Users.Add(user);
        _context.SaveChanges();

        return Result<UserGetResponse>.Success(ToGetResponse(user));
    }

    public Result<List<UserGetAllResponse>> GetAllUsers()
    {
        var users = _context.Users.AsNoTracking().ToList();

        return Result<List<UserGetAllResponse>>.Success(users.Select(ToGetAllResponse).ToList());
    }

    public Result UpdateUser(int userId, string fullName, string phoneNumber, string address)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return Result.Failure("User didn't find !!", ResultErrorType.NotFound);
        }

        var request = new UserUpdateRequest(fullName, phoneNumber, address);

        var validation = request.Validate();

        if (!validation.IsValid)
        {
            return Result.Failure(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)), ResultErrorType.BadRequest);
        }

        user.UpdateInformation(fullName, phoneNumber, address);
        _context.SaveChanges();

        return Result.Success();
    }

    public Result DeleteUser(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return Result.Failure("User didn't find !!", ResultErrorType.NotFound);
        }

        _context.Users.Remove(user);
        _context.SaveChanges();

        return Result.Success();
    }

    private static UserGetResponse ToGetResponse(User user) => new(user.Id, user.FullName, user.PhoneNumber, user.Address);

    private static UserGetAllResponse ToGetAllResponse(User user) => new(user.Id, user.FullName, user.PhoneNumber, user.Address);
}