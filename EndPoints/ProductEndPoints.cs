using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products");

        group.MapPost("", (ProductCreateRequest request, ProductServices service) => ToHttpResult(service.AddProduct(request.Name, request.Price, request.Description, request.Stock)));

        group.MapGet("", (ProductServices service) => ToHttpResult(service.GetAllProducts()));

        group.MapGet("/{id:int}", (int id, ProductServices service) => ToHttpResult(service.GetProduct(id)));

        group.MapPut("/{id:int}", (int id, ProductUpdateRequest request, ProductServices service) => ToHttpResult(service.UpdateProduct(id, request.Name, request.Price, request.Description)));

        group.MapDelete("/{id:int}", (int id, ProductServices service) => ToHttpResult(service.DeleteProduct(id)));

        group.MapGet("/search", (string search, ProductServices service) => ToHttpResult(service.SearchProducts(search)));

        group.MapGet("/paging", (int page, int pageSize, ProductServices service) => ToHttpResult(service.GetProducts(page, pageSize)));

        group.MapGet("/filter", (decimal? minPrice, decimal? maxPrice, ProductServices service) => ToHttpResult(service.FilterProducts(minPrice, maxPrice)));

        group.MapGet("/sort", (bool ascending, ProductServices service) => ToHttpResult(service.SortProducts(ascending)));

        return app;
    }

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        return result.ErrorType switch
        {
            ResultErrorType.BadRequest => TypedResults.BadRequest(new { error = result.Error }),
            ResultErrorType.NotFound => TypedResults.NotFound(new { error = result.Error }),
            ResultErrorType.Conflict => TypedResults.Conflict(new { error = result.Error }),
            _ => TypedResults.Problem(result.Error)
        };
    }

    private static IResult ToHttpResult(Result result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(new { message = "Operation completed successfully" });
        }

        return result.ErrorType switch
        {
            ResultErrorType.BadRequest => TypedResults.BadRequest(new { error = result.Error }),
            ResultErrorType.NotFound => TypedResults.NotFound(new { error = result.Error }),
            ResultErrorType.Conflict => TypedResults.Conflict(new { error = result.Error }),
            _ => TypedResults.Problem(result.Error)
        };
    }
}