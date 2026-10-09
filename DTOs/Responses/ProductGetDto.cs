namespace Shahbazi.Store.DTOs.Responses
{
    public class ProductGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; }
        public string? Description { get; set; } 
        public int Stock { get; set; }
    }
}