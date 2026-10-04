using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.ResultPattern;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs;
using Shahbazi.Store.Models;
using Shahbazi.Store.DTOs.Responses;

namespace Shahbazi.Store.Services;

public class ProductServices
{
    private readonly AppDbContext _context;
    public ProductServices(AppDbContext context)
    {
        _context = context;
    }
    public Result<Product> AddProduct(string name, decimal price, string? description, int stock)
    {
        var productResult = Product.Create(name, price, description, stock);
        if (!productResult.IsSuccess)
        {
            return productResult;
        }
        var product = productResult.Value!;
        _context.Products.Add(product);
        _context.SaveChanges();
        return Result<Product>.Success(product);
    }
    public Result<List<Product>> GetAllProducts()
    {
        return Result<List<Product>>.Success(_context.Products.ToList());
    }
    public Result<List<Product>> SearchProducts(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return GetAllProducts();
        }
        return Result<List<Product>>.Success(_context.Products.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search))).ToList());
    }
    public Result<PagedResult<Product>> GetProducts(int page, int pageSize)
    {
        if (page < 1)
        {
            return Result<PagedResult<Product>>.Failure("Page must be greater than 0", ResultErrorType.BadRequest);
        }
        if (pageSize < 1)
        {
            return Result<PagedResult<Product>>.Failure("PageSize most be greater than 0", ResultErrorType.BadRequest);
        }
        var totalItems = _context.Products.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        var items = _context.Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Result<PagedResult<Product>>.Success(
            new PagedResult<Product>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            });
    }
    public Result<Product> GetProduct(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return Result<Product>.Failure("Product didn't find !!", ResultErrorType.NotFound);
        }
        return Result<Product>.Success(product);
    }

    public Result<ProductGetResponse> GetProductResponse(int id)
    {
        var product = _context.Products.SingleOrDefault(p => p.Id == id);
        if (product is null)
        {
            return Result<ProductGetResponse>.Failure("Product didn't find !!", ResultErrorType.NotFound);
        }

        return Result<ProductGetResponse>.Success(product.ToResponse());
    }

    public Result DeleteProduct(int id)
    {
        var productResult = GetProduct(id);
        if (!productResult.IsSuccess)
        {
            return Result.Failure(productResult.Error!, productResult.ErrorType);
        }
        _context.Products.Remove(productResult.Value!);
        _context.SaveChanges();
        return Result.Success();
    }
    public Result IncreaseStock(int productId, int amount)
    {
        var productResult = GetProduct(productId);
        if (!productResult.IsSuccess)
        {
            return Result.Failure(productResult.Error!, productResult.ErrorType);
        }
        var result = productResult.Value!.IncreaseStock(amount);
        if (!result.IsSuccess)
        {
            return result;
        }
        _context.SaveChanges();
        return Result.Success();
    }
    public Result DecreaseStock(int productId, int amount)
    {
        var productResult = GetProduct(productId);
        if (!productResult.IsSuccess)
        {
            return Result.Failure(productResult.Error!, productResult.ErrorType);
        }
        var result = productResult.Value!.DecreaseStock(amount);
        if (!result.IsSuccess)
        {
            return result;
        }
        _context.SaveChanges();
        return Result.Success();
    }
    public Result UpdateProduct(int id, string name, decimal price, string description)
    {
        var productResult = GetProduct(id);
        if (!productResult.IsSuccess)
        {
            return Result.Failure(productResult.Error!, productResult.ErrorType);
        }
        var result = productResult.Value!.UpdateInformation(name, price, description);
        if (!result.IsSuccess)
        {
            return result;
        }
        _context.SaveChanges();
        return Result.Success();
    }
    public Result<List<Product>> FilterProducts(decimal? minPrice, decimal? maxPrice)
    {
        var products = _context.Products.AsEnumerable();
        if (minPrice.HasValue)
        {
            products = products.Where(p => p.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= maxPrice.Value);
        }
        return Result<List<Product>>.Success(products.ToList());
    }
    public Result<List<Product>> SortProducts(bool ascending)
    {
        if (ascending)
        {
            return Result<List<Product>>.Success(_context.Products.OrderBy(p => p.Price).ToList());
        }
        return Result<List<Product>>.Success(_context.Products.OrderByDescending(p => p.Price).ToList());
    }
}