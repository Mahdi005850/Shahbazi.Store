using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Models;
using System.Linq.Expressions;
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

        return Result<ProductGetResponse>.Success(product.ToGetResponse());
    }

    public async Task<Result<ProductGetResponse>> GetProductAsync(int id)
    {
        if (id <= 0)
        {
            return Result<ProductGetResponse>.BadRequestError("Product id is invalid");
        }

        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return Result<ProductGetResponse>.NotFoundError("Product didn't find !!");
        }

        return Result<ProductGetResponse>.Success(product.ToGetResponse());
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
            return Result.NotFoundError("Product didn't find !!");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateProductAsync(int id, string name, decimal price, string? description)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return Result.NotFoundError("Product didn't find !!");
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
        return Result.Success();
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
            return Result.NotFoundError("Product didn't find !!");
        }

        var result = increase ? product.IncreaseStock(amount) : product.DecreaseStock(amount);
        if (!result.IsSuccess) return result;

        await _context.SaveChangesAsync();
        return Result.Success();
    }

    private static readonly Dictionary<string, Expression<Func<Product, object>>> SortableColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["name"] = p => p.Name,
        ["price"] = p => p.Price,
        ["stock"] = p => p.Stock
    };

    public async Task<Result<PagedResult<ProductGetAllResponse>>> GetProductsAsync(ProductQuery query)
    {
        var validation = query.Validate();

        if (!validation.IsValid)
        {
            return Result<PagedResult<ProductGetAllResponse>>.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        if (query.SortBy is not null && !SortableColumns.ContainsKey(query.SortBy))
        {
            return Result<PagedResult<ProductGetAllResponse>>.BadRequestError($"SortBy must be one of: {string.Join(", ", SortableColumns.Keys)}.");
        }

        var products = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            products = products.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));
        }

        if (query.MinPrice.HasValue) products = products.Where(p => p.Price >= query.MinPrice.Value);
        if (query.MaxPrice.HasValue) products = products.Where(p => p.Price <= query.MaxPrice.Value);

        var paged = await products.ApplySort(query.SortBy, query.Descending, SortableColumns, p => p.Id).ToPagedResultAsync(query.Page, query.PageSize, p => p.ToGetAllResponse());

        return Result<PagedResult<ProductGetAllResponse>>.Success(paged);
    }
}