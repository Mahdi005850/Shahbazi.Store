namespace Shahbazi.Store.Models;

public class User
{
    public int Id { get; private set; }
    public string FullName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Address { get; private set; }
    public ICollection<Cart> Carts { get; private set; } = new List<Cart>();
    public ICollection<Order> Orders { get; private set; } = new List<Order>();
    public User(string fullName, string phoneNumber, string address)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
    }
    public void UpdateInformation(string fullName, string phoneNumber, string address)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
    }
}