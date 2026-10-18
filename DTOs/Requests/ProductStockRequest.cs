using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record ProductStockRequest(int Amount);

internal static class ProductStockRequestExtensions
{
    public static ValidationResult Validate(this ProductStockRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (request.Amount <= 0)
        {
            errors.Add(new(nameof(request.Amount), "Amount must be greater than zero."));
        }

        return new ValidationResult(errors.ToArray());
    }
}