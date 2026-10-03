using Shahbazi.Store.Data;
using Shahbazi.Store.Models;
using Shahbazi.Store.ResultPattern;

namespace Shahbazi.Store.Services;

public class UserServices
{
    private readonly AppDbContext _context;
    public UserServices(AppDbContext context)
    {
        _context = context;
    }
    public Result<User> GetUser(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return Result<User>.Failure("User didn't finnd !!", ResultErrorType.NotFound);
        }
        return Result<User>.Success(user);
    }
    public Result<User> AddUser(string fullName, string phoneNumber, string address)
    {
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(address))
        {
            return Result<User>.Failure("User information cannot be empty !!", ResultErrorType.BadRequest);
        }
        var user = new User(fullName, phoneNumber, address);
        _context.Users.Add(user);
        _context.SaveChanges();
        return Result<User>.Success(user);
    }
    public Result<List<User>> GetAllUsers()
    {
        return Result<List<User>>.Success(_context.Users.ToList());
    }
    public Result UpdateUser(int userId, string fullName, string phoneNumber, string adddress)
    {
        var userResult = GetUser(userId);
        if (!userResult.IsSuccess)
        {
            return Result.Failure(userResult.Error!, userResult.ErrorType);
        }
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(adddress))
        {
            return Result.Failure("User information cannot be empty !!", ResultErrorType.BadRequest);
        }
        userResult.Value!.UpdateInformation(fullName, phoneNumber, adddress);
        _context.SaveChanges();
        return Result.Success();
    }
    public Result DeleteUser(int userId)
    {
        var userResult = GetUser(userId);
        if (!userResult.IsSuccess)
        {
            return Result.Failure(userResult.Error!, userResult.ErrorType);
        }
        _context.Users.Remove(userResult.Value!);
        _context.SaveChanges();
        return Result.Success();
    }
}