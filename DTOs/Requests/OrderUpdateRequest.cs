using Shahbazi.Store.Common;

namespace Shahbazi.Store.DTOs.Requests;

public record OrderUpdateRequest(string ShippingAddress);

internal static class OrderUpdateRequestExtensions
{
    public static ValidationResult Validate(this OrderUpdateRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (string.IsNullOrWhiteSpace(request.ShippingAddress))
        {
            errors.Add(new(nameof(request.ShippingAddress), "Shipping address is required."));
        }

        return new ValidationResult(errors.ToArray());
    }
}