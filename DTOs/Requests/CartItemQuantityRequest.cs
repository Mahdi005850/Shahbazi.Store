using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record CartItemQuantityRequest(int Amount);

internal static class CartItemQuantityRequestExtensions
{
    public static ValidationResult Validate(this CartItemQuantityRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (request.Amount <= 0)
        {
            errors.Add(new(nameof(request.Amount), "Amount must be greater than zero."));
        }

        return new ValidationResult(errors.ToArray());
    }
}