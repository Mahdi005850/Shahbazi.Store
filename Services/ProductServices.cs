using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Resources;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Models;
using Shahbazi.Store.Common.Querying;
using Shahbazi.Store.DTOs.Queries;

namespace Shahbazi.Store.Services;

public class ProductServices
{
    private readonly AppDbContext _context;

    public ProductServices(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductGetResponse>> AddProductAsync(string name, decimal price, string? description, int stock)
    {
        var request = new ProductCreateRequest(name, price, description, stock);
        var validation = request.Validate();

        if (!validation.IsValid)
        {
            return Result<ProductGetResponse>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var productResult = Product.Create(name, price, description, stock);

        if (!productResult.IsSuccess)
        {
            return productResult.ToFailure<ProductGetResponse>();
        }

        var product = productResult.Value!;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Result<ProductGetResponse>.Success(product.ToGetResponse(), "Product created successfully.");
    }

    public async Task<Result<ProductGetResponse>> GetProductAsync(int id)
    {
        if (id <= 0)
        {
            return Result<ProductGetResponse>.InvalidId("Product");
        }

        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return Result<ProductGetResponse>.NotFound("Product");
        }

        return Result<ProductGetResponse>.Success(product.ToGetResponse(), "Product retrieved successfully.");
    }

    public async Task<Result> DeleteProductAsync(int id)
    {
        var validation = new ProductDeleteRequest(id).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return Result.NotFound("Product");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Result.Success("Product deleted successfully.");
    }

    public async Task<Result> UpdateProductAsync(int id, string name, decimal price, string? description)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return Result.NotFound("Product");
        }

        var request = new ProductUpdateRequest(name, price, description);
        var validation = request.Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var result = product.UpdateInformation(name, price, description);

        if (!result.IsSuccess)
        {
            return result;
        }

        await _context.SaveChangesAsync();
        return Result.Success("Product updated successfully.");
    }

    public Task<Result> IncreaseStockAsync(int productId, int amount) => ChangeStockAsync(productId, amount, true);

    public Task<Result> DecreaseStockAsync(int productId, int amount) => ChangeStockAsync(productId, amount, false);

    private async Task<Result> ChangeStockAsync(int productId, int amount, bool increase)
    {
        var validation = new ProductStockRequest(amount).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
        {
            return Result.NotFound("Product");
        }

        var result = increase ? product.IncreaseStock(amount) : product.DecreaseStock(amount);
        if (!result.IsSuccess) return result;

        await _context.SaveChangesAsync();
        return Result.Success(increase ? "Product stock increased successfully." : "Product stock decreased successfully.");
    }
    public async Task<Result<List<ProductGetAllResponse>>> GetProductsAsync(ProductQuery query)
    {
        var validation = query.Validate();

        if (!validation.IsValid)
        {
            return Result<List<ProductGetAllResponse>>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var productsQuery = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            productsQuery = productsQuery.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));
        }

        if (query.MinPrice.HasValue) productsQuery = productsQuery.Where(p => p.Price >= query.MinPrice.Value);
        if (query.MaxPrice.HasValue) productsQuery = productsQuery.Where(p => p.Price <= query.MaxPrice.Value);

        var products = await productsQuery
            .OrderBy(p => p.Id)
            .Page(query.Page, query.PageSize)
            .ToListAsync();

        var items = products.Select(p => p.ToGetAllResponse()).ToList();

        return Result<List<ProductGetAllResponse>>.Success(items, Messages.ProductsRetrieved);
    }
}