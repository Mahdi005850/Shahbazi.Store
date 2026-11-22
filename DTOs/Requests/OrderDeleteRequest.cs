using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record OrderDeleteRequest(int Id);

internal static class OrderDeleteRequestExtensions
{
    public static ValidationResult Validate(this OrderDeleteRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (request.Id <= 0)
        {
            errors.Add(new(nameof(request.Id), "Order id is invalid."));
        }

        return new ValidationResult(errors.ToArray());
    }
}