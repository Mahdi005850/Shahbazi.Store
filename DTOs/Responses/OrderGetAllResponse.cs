using Shahbazi.Store.Enums;

namespace Shahbazi.Store.DTOs.Responses;

public record OrderGetAllResponse(int Id, int UserId, DateTime CreatedAt, OrderStatus Status, string ShippingAddress, decimal TotalPrice);