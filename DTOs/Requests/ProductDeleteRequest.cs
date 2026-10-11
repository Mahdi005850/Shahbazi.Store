using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record ProductDeleteRequest(int Id);

internal static class ProductDeleteRequestExtensions
{
    public static ValidationResult Validate(this ProductDeleteRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (request.Id <= 0)
        {
            errors.Add(new(nameof(request.Id), "Id must be greater than zero."));
        }

        return new ValidationResult(errors.ToArray());
    }
}