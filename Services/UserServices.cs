using Shahbazi.Store.Data;
using Shahbazi.Store.Models;
namespace Shahbazi.Store.Services;

public class UserServices
{
    private readonly AppDbContext _context;
    public UserServices(AppDbContext context)
    {
        _context = context;
    }
    public User? GetUser(int userId)
    {
        return _context.Users.FirstOrDefault(u => u.Id == userId);
    }
    public void AddUser(string fullName, string phoneNumber, string address)
    {
        var user = new User(fullName, phoneNumber, address);
        _context.Users.Add(user);
        _context.SaveChanges();
    }
    public List<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }
    public void UpdateUser(int userId, string fullName, string phoneNumber, string adddress)
    {
        var user = GetUser(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User didn't finnd !!");
        }
        user.UpdateInformation(fullName, phoneNumber, adddress);
        _context.SaveChanges();
    }
    public void DeleteUser(int userId)
    {
        var user = GetUser(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User didn't find !!");
        }
        _context.Users.Remove(user);
    }
}