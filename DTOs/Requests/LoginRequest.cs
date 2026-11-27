using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record LoginRequest(string PhoneNumber, string Password);

internal static class LoginRequestExtensions
{
    public static ValidationResult Validate(this LoginRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            errors.Add(new(nameof(request.PhoneNumber), "Phone number is required."));
        }

        if (string.IsNullOrEmpty(request.Password))
        {
            errors.Add(new(nameof(request.Password), "Password is required."));
        }

        return new ValidationResult(errors.ToArray());
    }
}