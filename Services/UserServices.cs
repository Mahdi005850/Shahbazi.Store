using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common;
using Shahbazi.Store.Common.Querying;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.DTOs.Responses;
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
            return Result<UserGetResponse>.BadRequestError("User id is invalid");
        }

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Result<UserGetResponse>.NotFoundError("User didn't find !!");
        }

        return Result<UserGetResponse>.Success(user.ToGetResponse());
    }

    public async Task<Result<UserGetResponse>> AddUserAsync(string fullName, string phoneNumber, string address)
    {
        var request = new UserCreateRequest(fullName, phoneNumber, address);
        var validation = request.Validate();

        if (!validation.IsValid)
        {
            return Result<UserGetResponse>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var user = new User(fullName, phoneNumber, address);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Result<UserGetResponse>.Success(user.ToGetResponse());
    }

    public async Task<Result> UpdateUserAsync(int userId, string fullName, string phoneNumber, string address)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Result.NotFoundError("User didn't find !!");
        }

        var request = new UserUpdateRequest(fullName, phoneNumber, address);

        var validation = request.Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        user.UpdateInformation(fullName, phoneNumber, address);
        await _context.SaveChangesAsync();

        return Result.Success();
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
            return Result.NotFoundError("User didn't find !!");
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    private static readonly Dictionary<string, Expression<Func<User, object>>> SortableColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["fullName"] = u => u.FullName,
        ["phoneNumber"] = u => u.PhoneNumber
    };

    public async Task<Result<PagedResult<UserGetAllResponse>>> GetUsersAsync(UserQuery query)
    {
        var validation = query.Validate();

        if (!validation.IsValid)
        {
            return Result<PagedResult<UserGetAllResponse>>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        if (query.SortBy is not null && !SortableColumns.ContainsKey(query.SortBy))
        {
            return Result<PagedResult<UserGetAllResponse>>.BadRequestError($"SortBy must be one of: {string.Join(", ", SortableColumns.Keys)}.");
        }

        var users = _context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            users = users.Where(u => u.FullName.Contains(search) || u.PhoneNumber.Contains(search));
        }

        var paged = await users.ApplySort(query.SortBy, query.Descending, SortableColumns, u => u.Id).ToPagedResultAsync(query.Page, query.PageSize, u => u.ToGetAllResponse());

        return Result<PagedResult<UserGetAllResponse>>.Success(paged);
    }
}