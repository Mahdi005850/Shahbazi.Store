using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products (public)");

        group.MapGet("", async ([AsParameters] ProductQuery query, ProductServices service) => ToHttpResult(await service.GetProductsAsync(query)));

        group.MapGet("/{id:int}", async (int id, ProductServices service) => ToHttpResult(await service.GetProductAsync(id)));

        return app;
    }
}