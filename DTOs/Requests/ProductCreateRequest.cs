using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Requests;

public record ProductCreateRequest(
    string Name,
    decimal Price,
    string? Description,
    int Stock
);

internal static class ProductCreateRequestExtensions
{
    public static Product ToModel(this ProductCreateRequest request)
    {
        return new Product(request.Name, request.Price, request.Description, request.Stock);
    }
    
    public static ValidationResult Validate(this ProductCreateRequest request)
    {
        var errors = new List<ValidationResult.ValidationError>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add(new ValidationResult.ValidationError(nameof(request.Name), "Name is required."));
        }

        if (request.Price <= 0)
        {
            errors.Add(new ValidationResult.ValidationError(nameof(request.Price), "Price must be greater than zero."));
        }

        if (request.Stock < 0)
        {
            errors.Add(new ValidationResult.ValidationError(nameof(request.Stock), "Stock cannot be negative."));
        }

        return new ValidationResult(errors.ToArray());
    }
}

public class ValidationResult
{
    public class ValidationError
    {
        public string PropertyName { get; }
        public string ErrorMessage { get; }

        public ValidationError(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
    }

    public bool IsValid => Errors.Length == 0;
    public ValidationError[] Errors { get; }

    public ValidationResult(ValidationError[] error)
    {
        Errors = error;
    }
}