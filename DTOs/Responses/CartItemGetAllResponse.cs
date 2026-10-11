namespace Shahbazi.Store.DTOs.Responses;

public record CartItemGetAllResponse(int Id, int ProductId, int Quantity, decimal UnitPrice, decimal TotalPrice);
