using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Responses;

public record UserGetAllResponse(int Id, string FullName, string PhoneNumber, string Address);

public static class UserGetAllResponseExtensions
{
    public static UserGetAllResponse ToResponse(this User user)
    {
        return new UserGetAllResponse(user.Id, user.FullName, user.PhoneNumber, user.Address);
    }
}