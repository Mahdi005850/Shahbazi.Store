using System.ComponentModel.DataAnnotations;

namespace Shahbazi.Store.DTOs.Requests;

public class ProductUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
}