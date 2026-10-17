using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using static Shahbazi.Store.Common.CodeResponse.ApiResults;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products");

        group.MapPost("", (ProductCreateRequest request, ProductServices service) => ToHttpResult(service.AddProduct(request.Name, request.Price, request.Description, request.Stock), new(Success: "Product created successfully.")));

        group.MapGet("", (ProductServices service) => ToHttpResult(service.GetAllProducts(), new(Success: "Products retrieved successfully.")));

        group.MapGet("/{id:int}", (int id, ProductServices service) => ToHttpResult(service.GetProduct(id), new(Success: "Product retrieved successfully.", BadRequest: "Product id is invalid.", NotFound: "Product not found.")));

        group.MapPut("/{id:int}", (int id, ProductUpdateRequest request, ProductServices service) => ToHttpResult(service.UpdateProduct(id, request.Name, request.Price, request.Description), new(Success: "Product updated successfully.", NotFound: "Product not found.")));

        group.MapDelete("/{id:int}", (int id, ProductServices service) => ToHttpResult(service.DeleteProduct(id), new(Success: "Product deleted successfully.", NotFound: "Product not found.")));

        group.MapGet("/search", (string search, ProductServices service) => ToHttpResult(service.SearchProducts(search), new(Success: "Products retrieved successfully.")));

        group.MapGet("/paging", (int page, int pageSize, ProductServices service) => ToHttpResult(service.GetProducts(page, pageSize), new(Success: "Products retrieved successfully.")));

        group.MapGet("/filter", (decimal? minPrice, decimal? maxPrice, ProductServices service) => ToHttpResult(service.FilterProducts(minPrice, maxPrice), new(Success: "Products filtered successfully.")));

        group.MapGet("/sort", (bool ascending, ProductServices service) => ToHttpResult(service.SortProducts(ascending), new(Success: "Products sorted successfully.")));

        return app;
    }
}