using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Responses;

public record ProductGetResponse (
    int Id,
    string Name,
    string Price,
    string? Description,
    int Stock
);

public static class ProductGetResponseExtensions
{
    public static ProductGetResponse ToResponse(this Product product)
    {
        return new ProductGetResponse(
            product.Id,
            product.Name,
            product.Price.ToString("F2"),
            product.Description,
            product.Stock
        );
    }
}