using Shahbazi.Store.Enums;

namespace Shahbazi.Store.Models;

public class User
{
    public int Id { get; private set; }
    public string FullName { get; private set; }
    public string PhoneNumber { get; private set; }   // this is also the UserName
    public string Address { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    public User(string fullName, string phoneNumber, string address, UserRole role = UserRole.Buyer)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
        Role = role;
    }

    public void UpdateInformation(string fullName, string phoneNumber, string address)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }
}