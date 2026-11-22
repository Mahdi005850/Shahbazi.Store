namespace Shahbazi.Store.Common.Querying;

public static class QueryableExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int pageSize)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}