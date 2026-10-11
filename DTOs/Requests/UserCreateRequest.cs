using Microsoft.AspNetCore.Identity;
using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record UserCreateRequest(string FullName, string PhoneNumber, string Address);

internal static class UserCreateRequestExtensions
{
    public static ValidationResult Validate(this UserCreateRequest request)
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
     
        if (string.IsNullOrWhiteSpace(request.Address))
        {
            errors.Add(new(nameof(request.Address), "Address is required."));
        }

        return new ValidationResult(errors.ToArray());
    }
}