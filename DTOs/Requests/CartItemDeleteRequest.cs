using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record CartItemDeleteRequest(int ProductId);

internal static class CartItemDeleteRequestExtensions
{
    public static ValidationResult Validate(this CartItemDeleteRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (request.ProductId <= 0)
        {
            errors.Add(new(nameof(request.ProductId), "ProductId must be greater than zero."));
        }

        return new ValidationResult(errors.ToArray());
    }
}