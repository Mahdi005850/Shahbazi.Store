using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Responses;

public record UserGetResponse(int Id, string FullName, string PhoneNumber, string Address, UserRole Role);

public static class UserGetResponseExtensions
{
    public static UserGetResponse ToGetResponse(this User user)
    {
        return new UserGetResponse(user.Id, user.FullName, user.PhoneNumber, user.Address, user.Role);
    }
}