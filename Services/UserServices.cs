using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common;
using Shahbazi.Store.Common.Querying;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;
using System.Linq.Expressions;

namespace Shahbazi.Store.Services;

public class UserServices
{
    private readonly AppDbContext _context;

    public UserServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserGetResponse>> GetUserAsync(int userId)
    {
        if (userId <= 0)
        {
            return Result<UserGetResponse>.InvalidId("User");
        }

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Result<UserGetResponse>.NotFound("User");
        }

        return Result<UserGetResponse>.Success(user.ToGetResponse(), "User retrieved successfully.");
    }

    public async Task<Result> UpdateUserAsync(int userId, string fullName, string phoneNumber, string address)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Result.NotFound("User");
        }

        var request = new UserUpdateRequest(fullName, phoneNumber, address);

        var validation = request.Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var newPhoneNumber = phoneNumber.Trim();

        if (await _context.Users.AnyAsync(u => u.PhoneNumber == newPhoneNumber && u.Id != userId))
        {
            return Result.ConflictError("A user with this phone number already exists.");
        }

        user.UpdateInformation(fullName.Trim(), newPhoneNumber, address.Trim());
        await _context.SaveChangesAsync();

        return Result.Success("User updated successfully.");
    }

    public async Task<Result> ChangeRoleAsync(int userId, UserRole role)
    {
        if (!Enum.IsDefined(role))
        {
            return Result.BadRequestError("Role is invalid.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Result.NotFound("User");
        }

        user.ChangeRole(role);
        await _context.SaveChangesAsync();

        return Result.Success("User role updated successfully.");
    }

    public async Task<Result> DeleteUserAsync(int userId)
    {
        var validation = new UserDeleteRequest(userId).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Result.NotFound("User");
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return Result.Success("User deleted successfully.");
    }

    public async Task<Result<List<UserGetAllResponse>>> GetUsersAsync(UserQuery query)
    {
        var validation = query.Validate();

        if (!validation.IsValid)
        {
            return Result<List<UserGetAllResponse>>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var usersQuery = _context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            usersQuery = usersQuery.Where(u => u.FullName.Contains(search) || u.PhoneNumber.Contains(search));
        }

        var users = await usersQuery
            .OrderBy(u => u.Id)
            .Page(query.Page, query.PageSize)
            .ToListAsync();

        var items = users.Select(u => u.ToGetAllResponse()).ToList();

        return Result<List<UserGetAllResponse>>.Success(items, "Users retrieved successfully.");
    }
}