using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Responses;

public record UserGetResponse(int Id, string FullName, string PhoneNumber, string Address);

public static class UserGetResponseExtensions
{
    public static UserGetResponse ToResponse(this User user)
    {
        return new UserGetResponse(user.Id, user.FullName, user.PhoneNumber, user.Address);
    }
}