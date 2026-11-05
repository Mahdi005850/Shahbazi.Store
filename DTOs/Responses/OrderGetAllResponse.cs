using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Responses;

public record OrderGetAllResponse(int Id, int UserId, DateTime CreatedAt, OrderStatus Status, string ShippingAddress, decimal TotalPrice); public static class OrderGetAllResponseExtensions
{
    public static OrderGetAllResponse ToGetAllResponse(this Order order)
    {
        return new OrderGetAllResponse(order.Id, order.UserId, order.CreatedAt, order.Status, order.ShippingAddress, order.OrderItems.Sum(item => item.TotalPrice));
    }
}