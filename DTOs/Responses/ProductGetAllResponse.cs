using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Responses;

public record ProductGetAllResponse(int Id, string Name, string Price, string? Description, int Stock);

public static class ProductGetAllResponseExtensions
{
    public static ProductGetAllResponse ToResponse(this Product product)
    {
        return new ProductGetAllResponse(product.Id, product.Name, product.Price.ToString("F2"), product.Description, product.Stock);
    }
}