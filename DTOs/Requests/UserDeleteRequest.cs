using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record UserDeleteRequest(int Id);

internal static class UserDeleteRequestExtensions
{
    public static ValidationResult Validate(this UserDeleteRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (request.Id <= 0)
        {
            errors.Add(new(nameof(request.Id), "User id is invalid."));
        }

        return new ValidationResult(errors.ToArray());
    }
}