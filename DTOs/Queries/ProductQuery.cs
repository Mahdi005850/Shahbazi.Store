using Shahbazi.Store.Common;
using Shahbazi.Store.Common.Querying;

namespace Shahbazi.Store.DTOs.Queries;

public record ProductQuery(string? Search = null, decimal? MinPrice = null, decimal? MaxPrice = null, string? SortBy = null, bool Descending = false, int Page = 1, int PageSize = 10);

internal static class ProductQueryExtensions
{
    public static ValidationResult Validate(this ProductQuery query)
    {
        var errors = new List<ValidationResult.ValidationError>();

        PagingRules.Validate(query.Page, query.PageSize, errors);

        if (query.MinPrice.HasValue && query.MaxPrice.HasValue && query.MinPrice > query.MaxPrice)
        {
            errors.Add(new(nameof(query.MinPrice), "Minimum price cannot be greater than maximum price."));
        }

        return new ValidationResult(errors.ToArray());
    }
}