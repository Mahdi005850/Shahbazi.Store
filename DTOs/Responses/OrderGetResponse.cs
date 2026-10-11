using Shahbazi.Store.Enums;
using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Responses;

public record OrderGetResponse(int Id, int UserId, DateTime CreatedAt, OrderStatus Status, string ShippingAddress, decimal TotalPrice, List<OrderItemGetResponse> OrderItems);

public static class OrderGetResponseExtensions
{
    public static OrderGetResponse ToResponse(this Order order)
    {
        var items = order.OrderItems.Select(item => new OrderItemGetResponse(item.Id, item.ProductId, item.Quantity, item.UnitPrice, item.TotalPrice)).ToList();

        return new OrderGetResponse(order.Id, order.UserId, order.CreatedAt, order.Status, order.ShippingAddress, items.Sum(item => item.TotalPrice), items);
    }
}