using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record CartItemCreateRequest(int ProductId, int Quantity
);

internal static class CartItemCreateRequestExtensions
{
    public static ValidationResult Validate(this CartItemCreateRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (request.ProductId <= 0)
        {
            errors.Add(new(nameof(request.ProductId), "ProductId must be greater than zero."));
        }

        if (request.Quantity <= 0)
        {
            errors.Add(new(nameof(request.Quantity), "Quantity must be greater than zero."));
        }

        return new ValidationResult(errors.ToArray());
    }
}