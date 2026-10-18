using Shahbazi.Store.Common;
using Shahbazi.Store.Common.Querying;

namespace Shahbazi.Store.DTOs.Queries;

public record UserQuery(string? Search = null, string? SortBy = null, bool Descending = false, int Page = 1, int PageSize = 10);

internal static class UserQueryExtensions
{
    public static ValidationResult Validate(this UserQuery query)
    {
        var errors = new List<ValidationResult.ValidationError>();

        PagingRules.Validate(query.Page, query.PageSize, errors);

        return new ValidationResult(errors.ToArray());
    }
}