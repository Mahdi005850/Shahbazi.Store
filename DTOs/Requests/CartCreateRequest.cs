using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record CartCreateRequest(int UserId);

internal static class CartCreateRequestExtensions
{
    public static ValidationResult Validate(this CartCreateRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (request.UserId <= 0)
        {
            errors.Add(new(nameof(request.UserId), "UserId must be greater than zero."));
        }

        return new ValidationResult(errors.ToArray());
    }
}