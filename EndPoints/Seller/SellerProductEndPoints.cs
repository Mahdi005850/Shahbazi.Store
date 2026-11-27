using Shahbazi.Store.Common.Auth;
using Shahbazi.Store.DTOs.Queries;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.Services;
using System.Security.Claims;
using static Shahbazi.Store.Common.ResultPattern.ApiResults;

public static class SellerProductEndpoints
{
    public static IEndpointRouteBuilder MapSellerProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/seller/products").RequireAuthorization(Policies.SellerOrAdmin).WithTags("Seller - Products");

        group.MapGet("", async ([AsParameters] ProductQuery query, ClaimsPrincipal user, ProductServices service) => ToHttpResult(await service.GetProductsAsync(query, user.IsAdmin() ? null : user.GetUserId())));

        group.MapPost("", async (ProductCreateRequest request, ClaimsPrincipal user, ProductServices service) => ToCreatedResult(await service.AddProductAsync(user.GetUserId(), request.Name, request.Price, request.Description, request.Stock), p => $"/api/products/{p.Id}"));

        var own = group.MapGroup("/{id:int}").AddEndpointFilter<ProductOwnerOrAdminFilter>();

        own.MapPut("", async (int id, ProductUpdateRequest request, ProductServices service) => ToHttpResult(await service.UpdateProductAsync(id, request.Name, request.Price, request.Description)));

        own.MapDelete("", async (int id, ProductServices service) => ToHttpResult(await service.DeleteProductAsync(id)));

        own.MapPatch("/stock/increase", async (int id, ProductStockRequest request, ProductServices service) => ToHttpResult(await service.IncreaseStockAsync(id, request.Amount)));

        own.MapPatch("/stock/decrease", async (int id, ProductStockRequest request, ProductServices service) => ToHttpResult(await service.DecreaseStockAsync(id, request.Amount)));

        return app;
    }
}