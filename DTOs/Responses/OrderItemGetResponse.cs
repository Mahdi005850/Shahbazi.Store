namespace Shahbazi.Store.DTOs.Responses;

public record OrderItemGetResponse(int Id, int ProductId, int Quantity, decimal UnitPrice, decimal TotalPrice);