using Shahbazi.Store.Common;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Requests;

public record ProductUpdateRequest(string Name, decimal Price, string? Description);

internal static class ProductUpdateRequestExtensions
{
    public static ValidationResult Validate(
        this ProductUpdateRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add(new(nameof(request.Name), "Name is required."));
        }

        if (request.Price <= 0)
        {
            errors.Add(new(
                nameof(request.Price),
                "Price must be greater than zero."
            ));
        }

        return new ValidationResult(errors.ToArray());
    }
    public static void ApplyTo(this ProductUpdateRequest request, Product product)
    {
        product.UpdateInformation(request.Name, request.Price, request.Description ?? string.Empty);
    }
}