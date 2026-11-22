using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products");

        group.MapPost("", async (ProductCreateRequest request, ProductServices service) => ToCreatedResult(await service.AddProductAsync(request.Name, request.Price, request.Description, request.Stock), p => $"/api/products/{p.Id}"));

        group.MapGet("", async ([AsParameters] ProductQuery query, ProductServices service) => ToHttpResult(await service.GetProductsAsync(query)));

        group.MapGet("/{id:int}", async (int id, ProductServices service) => ToHttpResult(await service.GetProductAsync(id)));

        group.MapPut("/{id:int}", async (int id, ProductUpdateRequest request, ProductServices service) => ToHttpResult(await service.UpdateProductAsync(id, request.Name, request.Price, request.Description)));

        group.MapDelete("/{id:int}", async (int id, ProductServices service) => ToHttpResult(await service.DeleteProductAsync(id)));

        group.MapPatch("/{id:int}/stock/increase", async (int id, ProductStockRequest request, ProductServices service) => ToHttpResult(await service.IncreaseStockAsync(id, request.Amount)));

        group.MapPatch("/{id:int}/stock/decrease", async (int id, ProductStockRequest request, ProductServices service) => ToHttpResult(await service.DecreaseStockAsync(id, request.Amount)));

        return app;
    }
}