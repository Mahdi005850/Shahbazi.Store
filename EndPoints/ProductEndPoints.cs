using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Resources;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.CodeResponse.ApiResults;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products");

        group.MapPost("", async (ProductCreateRequest request, ProductServices service) => ToCreatedResult(await service.AddProductAsync(request.Name, request.Price, request.Description, request.Stock), p => $"/api/products/{p.Id}", new(Success: "Product created successfully.")));

        group.MapGet("", async ([AsParameters] ProductQuery query, ProductServices service) => ToHttpResult(await service.GetProductsAsync(query), new(Success: Messages.ProductsRetrieved)));

        group.MapGet("/{id:int}", async (int id, ProductServices service) => ToHttpResult(await service.GetProductAsync(id), new(Success: "Product retrieved successfully.", BadRequest: "Product id is invalid.", NotFound: "Product not found.")));

        group.MapPut("/{id:int}", async (int id, ProductUpdateRequest request, ProductServices service) => ToHttpResult(await service.UpdateProductAsync(id, request.Name, request.Price, request.Description), new(Success: "Product updated successfully.", NotFound: "Product not found.")));

        group.MapDelete("/{id:int}", async (int id, ProductServices service) => ToHttpResult(await service.DeleteProductAsync(id), new(Success: "Product deleted successfully.", BadRequest: "Product id is invalid.", NotFound: "Product not found.")));

        group.MapPatch("/{id:int}/stock/increase", async (int id, ProductStockRequest request, ProductServices service) => ToHttpResult(await service.IncreaseStockAsync(id, request.Amount), new(Success: "Product stock increased successfully.", BadRequest: "Amount must be greater than zero.", NotFound: "Product not found.")));

        group.MapPatch("/{id:int}/stock/decrease", async (int id, ProductStockRequest request, ProductServices service) => ToHttpResult(await service.DecreaseStockAsync(id, request.Amount), new(Success: "Product stock decreased successfully.", BadRequest: "Amount must be greater than zero.", NotFound: "Product not found.", Conflict: "Not enough stock for this product.")));

        return app;
    }
}