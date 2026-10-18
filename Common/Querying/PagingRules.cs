namespace Shahbazi.Store.Common.Querying;

public static class PagingRules
{
    public const int MaxPageSize = 100;

    public static void Validate(int page, int pageSize, List<ValidationResult.ValidationError> errors)
    {
        if (page < 1)
        {
            errors.Add(new(nameof(page), "Page must be greater than 0."));
        }

        if (pageSize < 1 || pageSize > MaxPageSize)
        {
            errors.Add(new(nameof(pageSize), $"PageSize must be between 1 and {MaxPageSize}."));
        }
    }
}