namespace Shahbazi.Store.DTOs.Responses;

public record OrderItemGetAllResponse(int Id, int ProductId, int Quantity, decimal UnitPrice, decimal TotalPrice);