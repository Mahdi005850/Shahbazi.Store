using Shahbazi.Store.Common;
using Shahbazi.Store.Common.Querying;
using Shahbazi.Store.Enums;

namespace Shahbazi.Store.DTOs.Queries;

public record OrderQuery(int? UserId = null, OrderStatus? Status = null, DateTime? FromDate = null, DateTime? ToDate = null, string? SortBy = null, bool Descending = true, int Page = 1, int PageSize = 10);

internal static class OrderQueryExtensions
{
    public static ValidationResult Validate(this OrderQuery query)
    {
        var errors = new List<ValidationResult.ValidationError>();

        PagingRules.Validate(query.Page, query.PageSize, errors);

        if (query.FromDate.HasValue && query.ToDate.HasValue && query.FromDate > query.ToDate)
        {
            errors.Add(new(nameof(query.FromDate), "FromDate cannot be after ToDate."));
        }

        return new ValidationResult(errors.ToArray());
    }
}