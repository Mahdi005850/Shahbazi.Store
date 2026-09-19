using System.ComponentModel.DataAnnotations;
namespace Shahbazi.Store.DTOs;

public class CreateProductDto
{
    [Required]public string ProductName { get; set; } = string.Empty;
    [Range(0,double.MaxValue)]public decimal ProductPrice { get; set; }
    public string? ProductDescription { get; set; }
    [Range(0,int.MaxValue)]public int Stock { get; set; }
}