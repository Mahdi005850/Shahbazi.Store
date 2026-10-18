using System.Linq.Expressions;

namespace Shahbazi.Store.Common.Querying;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy, bool descending, Dictionary<string, Expression<Func<T, object>>> allowed, Expression<Func<T, object>> defaultKey)
    {
        if (sortBy is null || !allowed.TryGetValue(sortBy, out var key))
        {
            return descending ? query.OrderByDescending(defaultKey) : query.OrderBy(defaultKey);
        }

        var ordered = descending ? query.OrderByDescending(key) : query.OrderBy(key);

        return ordered.ThenBy(defaultKey);
    }
    public static PagedResult<TResult> ToPagedResult<TSource, TResult>(this IQueryable<TSource> query, int page, int pageSize, Func<TSource, TResult> map)
    {
        var totalItems = query.Count();

        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList().Select(map).ToList();

        return new PagedResult<TResult>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
        };
    }
}