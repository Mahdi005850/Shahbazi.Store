using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.CodeResponse.ApiResults;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products");

        group.MapPost("", (ProductCreateRequest request, ProductServices service) => ToCreatedResult(service.AddProduct(request.Name, request.Price, request.Description, request.Stock), p => $"/api/products/{p.Id}", new(Success: "Product created successfully.")));

        group.MapGet("", ([AsParameters] ProductQuery query, ProductServices service) => ToHttpResult(service.GetProducts(query), new(Success: "Products retrieved successfully.")));

        group.MapGet("/{id:int}", (int id, ProductServices service) => ToHttpResult(service.GetProduct(id), new(Success: "Product retrieved successfully.", BadRequest: "Product id is invalid.", NotFound: "Product not found.")));

        group.MapPut("/{id:int}", (int id, ProductUpdateRequest request, ProductServices service) => ToHttpResult(service.UpdateProduct(id, request.Name, request.Price, request.Description), new(Success: "Product updated successfully.", NotFound: "Product not found.")));

        group.MapDelete("/{id:int}", (int id, ProductServices service) => ToHttpResult(service.DeleteProduct(id), new(Success: "Product deleted successfully.", BadRequest: "Product id is invalid.", NotFound: "Product not found.")));

        group.MapPatch("/{id:int}/stock/increase", (int id, ProductStockRequest request, ProductServices service) => ToHttpResult(service.IncreaseStock(id, request.Amount), new(Success: "Product stock increased successfully.", BadRequest: "Amount must be greater than zero.", NotFound: "Product not found.")));

        group.MapPatch("/{id:int}/stock/decrease", (int id, ProductStockRequest request, ProductServices service) => ToHttpResult(service.DecreaseStock(id, request.Amount), new(Success: "Product stock decreased successfully.", BadRequest: "Amount must be greater than zero.", NotFound: "Product not found.", Conflict: "Not enough stock for this product.")));

        return app;
    }
}