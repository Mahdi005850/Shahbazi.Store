using Shahbazi.Store.Services;
using global::Shahbazi.Store.ResultPattern;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.DTOs.Requests;

namespace Shahbazi.Store.EndPoints;


internal static class ProductEndPoints
{
    public static void MapProductEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/products", (ProductCreateDto dto, ProductServices productServices) =>
        {
            var result = productServices.AddProduct(dto.Name, dto.Price, dto.Description, dto.Stock);
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok("Product Created Successfully !!");
        });
        app.MapGet("/api/products", (ProductServices productServices) =>
        {
            var result = productServices.GetAllProducts();
            return Results.Ok(result.Value);
        });
        app.MapGet("/api/products/search", (string search, ProductServices productServices) =>
        {
            var result = productServices.SearchProducts(search);
            return Results.Ok(result.Value);
        });
        app.MapGet("/api/products/paging", (int page, int pageSize, ProductServices productServices) =>
        {
            var result = productServices.GetProducts(page, pageSize);
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok(result.Value);
        });
        app.MapGet("/api/products/filter", (decimal? minPrice, decimal? maxPrice, ProductServices productServices) =>
        {
            var result = productServices.FilterProducts(minPrice, maxPrice);
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok(result.Value);
        });
        app.MapGet("/api/products/sort", (bool ascending, ProductServices productServices) =>
        {
            var result = productServices.SortProducts(ascending);
            return Results.Ok(result.Value);
        });
        app.MapGet("/api/products/{id}", (int id, ProductServices productServices) =>
        {
            var result = productServices.GetProduct(id);

            if (!result.IsSuccess)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
        app.MapPut("/api/products/{id}", (int id, ProductUpdateDto dto, ProductServices productServices) =>
        {
            var result = productServices.UpdateProduct(id, dto.Name, dto.Price, dto.Description);
            if (!result.IsSuccess)
            {
                if (result.ErrorType == ResultErrorType.NotFound)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.BadRequest(result.Error);
            }
            return Results.Ok("Product Updated Successfully !!");
        });
        app.MapDelete("/api/products/{id}", (int id, ProductServices productServices) =>
        {
            var result = productServices.DeleteProduct(id);
            if (!result.IsSuccess)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok("Product Deleted Successfully !!");
        });
    }
}