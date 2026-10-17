using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Common;
using Shahbazi.Store.Common.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs.Requests;
using Shahbazi.Store.DTOs.Responses;
using Shahbazi.Store.Models;

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

    public Result<List<ProductGetAllResponse>> GetAllProducts()
    {
        var products = _context.Products.AsNoTracking().ToList();

        return Result<List<ProductGetAllResponse>>.Success(products.Select(ToGetAllResponse).ToList());
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

    private static ProductGetResponse ToGetResponse(Product product) => new(product.Id, product.Name, product.Price.ToString("F2"), product.Description, product.Stock);

    private static ProductGetAllResponse ToGetAllResponse(Product product) => new(product.Id, product.Name, product.Price.ToString("F2"), product.Description, product.Stock);


    public Result<PagedResult<ProductGetAllResponse>> GetProducts(int page, int pageSize)
    {
        if (page < 1)
        {
            return Result<PagedResult<ProductGetAllResponse>>.BadRequestError("Page must be greater than 0");
        }

        if (pageSize < 1)
        {
            return Result<PagedResult<ProductGetAllResponse>>.BadRequestError("PageSize must be greater than 0");
        }

        var query = _context.Products.AsNoTracking().OrderBy(p => p.Id);

        var totalItems = query.Count();

        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList().Select(ToGetAllResponse).ToList();

        return Result<PagedResult<ProductGetAllResponse>>.Success(new PagedResult<ProductGetAllResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        });
    }

    public Result<List<ProductGetAllResponse>> SearchProducts(string search)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));
        }

        return Result<List<ProductGetAllResponse>>.Success(query.ToList().Select(ToGetAllResponse).ToList());
    }

    public Result<List<ProductGetAllResponse>> FilterProducts(decimal? minPrice, decimal? maxPrice)
    {
        if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
        {
            return Result<List<ProductGetAllResponse>>.BadRequestError("Minimum price cannot be greater than maximum price");
        }
        var query = _context.Products.AsNoTracking().AsQueryable();

        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);

        return Result<List<ProductGetAllResponse>>.Success(query.ToList().Select(ToGetAllResponse).ToList());
    }

    public Result<List<ProductGetAllResponse>> SortProducts(bool ascending)
    {
        var query = ascending ? _context.Products.AsNoTracking().OrderBy(p => p.Price) : _context.Products.AsNoTracking().OrderByDescending(p => p.Price);

        return Result<List<ProductGetAllResponse>>.Success(query.ToList().Select(ToGetAllResponse).ToList());
    }
}