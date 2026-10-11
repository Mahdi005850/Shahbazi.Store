using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record CartDeleteRequest(int UserId);

internal static class CartDeleteRequestExtensions
{
    public static ValidationResult Validate(this CartDeleteRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (request.UserId <= 0)
        {
            errors.Add(new(nameof(request.UserId), "UserId must be greater than zero."));
        }

        return new ValidationResult(errors.ToArray());
    }
}