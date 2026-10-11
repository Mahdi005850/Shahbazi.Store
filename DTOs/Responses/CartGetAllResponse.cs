namespace Shahbazi.Store.DTOs.Responses;

public record CartGetAllResponse(int Id, int UserId, DateTime CreatedAt, DateTime UpdatedAt, decimal TotalPrice);