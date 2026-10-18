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

    public Result<ProductGetResponse> AddProduct(string name, decimal price, string? description, int stock)
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
        _context.SaveChanges();

        return Result<ProductGetResponse>.Success(ToGetResponse(product));
    }

    public Result<ProductGetResponse> GetProduct(int id)
    {
        if (id <= 0)
        {
            return Result<ProductGetResponse>.BadRequestError("Product id is invalid");
        }

        var product = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);

        if (product is null)
        {
            return Result<ProductGetResponse>.NotFoundError("Product didn't find !!");
        }

        return Result<ProductGetResponse>.Success(ToGetResponse(product));
    }
    public Result DeleteProduct(int id)
    {
        var validation = new ProductDeleteRequest(id).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == id);

        if (product is null)
        {
            return Result.NotFoundError("Product didn't find !!");
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        return Result.Success();
    }
    public Result UpdateProduct(int id, string name, decimal price, string? description)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);

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

        _context.SaveChanges();
        return Result.Success();
    }

    public Result IncreaseStock(int productId, int amount) => ChangeStock(productId, amount, true);

    public Result DecreaseStock(int productId, int amount) => ChangeStock(productId, amount, false);

    private Result ChangeStock(int productId, int amount, bool increase)
    {
        var validation = new ProductStockRequest(amount).Validate();

        if (!validation.IsValid)
        {
            return Result.BadRequestError(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == productId);

        if (product is null)
        {
            return Result.NotFoundError("Product didn't find !!");
        }

        var result = increase ? product.IncreaseStock(amount) : product.DecreaseStock(amount);
        if (!result.IsSuccess) return result;

        _context.SaveChanges();
        return Result.Success();
    }
    private static readonly Dictionary<string, Expression<Func<Product, object>>> SortableColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["name"] = p => p.Name,
        ["price"] = p => p.Price,
        ["stock"] = p => p.Stock
    };

    public Result<PagedResult<ProductGetAllResponse>> GetProducts(ProductQuery query)
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

        var paged = products.ApplySort(query.SortBy, query.Descending, SortableColumns, p => p.Id).ToPagedResult(query.Page, query.PageSize, ToGetAllResponse);

        return Result<PagedResult<ProductGetAllResponse>>.Success(paged);
    }
    private static ProductGetResponse ToGetResponse(Product product) => new(product.Id, product.Name, product.Price.ToString("F2"), product.Description, product.Stock);

    private static ProductGetAllResponse ToGetAllResponse(Product product) => new(product.Id, product.Name, product.Price.ToString("F2"), product.Description, product.Stock);
}