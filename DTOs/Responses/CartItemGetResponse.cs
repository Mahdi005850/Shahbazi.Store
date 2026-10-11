namespace Shahbazi.Store.DTOs.Responses;

public record CartItemGetResponse(int Id, int ProductId, int Quantity, decimal UnitPrice, decimal TotalPrice);