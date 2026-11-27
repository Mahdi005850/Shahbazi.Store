using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record RegisterRequest(string FullName, string PhoneNumber, string Address, string Password);

internal static class RegisterRequestExtensions
{
    public static ValidationResult Validate(this RegisterRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            errors.Add(new(nameof(request.FullName), "Full name is required."));
        }

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            errors.Add(new(nameof(request.PhoneNumber), "Phone number is required."));
        }
        else if (request.PhoneNumber.Trim().Length > 20)
        {
            errors.Add(new(nameof(request.PhoneNumber), "Phone number cannot be longer than 20 characters."));
        }

        if (string.IsNullOrWhiteSpace(request.Address))
        {
            errors.Add(new(nameof(request.Address), "Address is required."));
        }

        if (string.IsNullOrEmpty(request.Password))
        {
            errors.Add(new(nameof(request.Password), "Password is required."));
        }
        else if (request.Password.Length < 8 || request.Password.Length > 100)
        {
            errors.Add(new(nameof(request.Password), "Password must be between 8 and 100 characters."));
        }

        return new ValidationResult(errors.ToArray());
    }
}