using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.Services;

public class ProductServices
{
    private readonly AppDbContext _context;

    public ProductServices(AppDbContext context)
    {
        _context = context;
    }

    public void AddProduct(
        string productName,
        decimal productPrice,
        string? productDescription,
        int stock)
    {
        var product = new Product(
            productName,
            productPrice,
            productDescription,
            stock);

        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public List<Product> GetAllProducts()
    {
        return _context.Products.ToList();
    }

    public List<Product> SearchProducts(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return GetAllProducts();
        }

        return _context.Products
            .Where(p =>
                p.ProductName.Contains(search) ||
                (p.ProductDescription != null &&
                 p.ProductDescription.Contains(search)))
            .ToList();
    }

    public PagedResult<Product> GetProducts(int page, int pageSize)
    {
        if (page < 1)
        {
            throw new ArgumentException("Page must be greater than 0");
        }

        if (pageSize < 1)
        {
            throw new ArgumentException("PageSize most be greater than 0");
        }

        var totalItems = _context.Products.Count();

        var totalPages =
            (int)Math.Ceiling((double)totalItems / pageSize);

        var items = _context.Products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Product>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public Product? GetProduct(int id)
    {
        return _context.Products
            .FirstOrDefault(p => p.Id == id);
    }

    public void DeleteProduct(int id)
    {
        var product = GetProduct(id);

        if (product == null)
        {
            throw new InvalidOperationException(
                "Product didn't find !!");
        }

        _context.Products.Remove(product);
        _context.SaveChanges();
    }

    public void IncreaseStock(int productId, int amount)
    {
        var product = GetProduct(productId);

        if (product == null)
        {
            throw new InvalidOperationException(
                "Product didn't find !!");
        }

        product.IncreaseStock(amount);
        _context.SaveChanges();
    }

    public void DecreaseStock(int productId, int amount)
    {
        var product = GetProduct(productId);

        if (product == null)
        {
            throw new InvalidOperationException(
                "Product didn't find !!");
        }

        product.DecreaseStock(amount);
        _context.SaveChanges();
    }

    public void UpdateProduct(
        int id,
        string productName,
        decimal productPrice,
        string productDescription)
    {
        var product = GetProduct(id);

        if (product == null)
        {
            throw new InvalidOperationException(
                "Product didn't find !!");
        }

        product.UpdateInformation(
            productName,
            productPrice,
            productDescription);

        _context.SaveChanges();
    }

    public List<Product> FilterProducts(
        decimal? minPrice,
        decimal? maxPrice)
    {
        var products = _context.Products.AsEnumerable();

        if (minPrice.HasValue)
        {
            products = products.Where(
                p => p.ProductPrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            products = products.Where(
                p => p.ProductPrice <= maxPrice.Value);
        }

        return products.ToList();
    }

    public List<Product> SortProducts(bool ascending)
    {
        if (ascending)
        {
            return _context.Products
                .OrderBy(p => p.ProductPrice)
                .ToList();
        }

        return _context.Products
            .OrderByDescending(p => p.ProductPrice)
            .ToList();
    }
}