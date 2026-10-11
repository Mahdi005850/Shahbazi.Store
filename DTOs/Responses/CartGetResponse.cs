using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Responses;

public record CartGetResponse(int Id, int UserId, DateTime CreatedAt, DateTime UpdatedAt, decimal TotalPrice, List<CartItemGetResponse> CartItems);

public static class CartGetResponseExtensions
{
    public static CartGetResponse ToResponse(this Cart cart)
    {
        return new CartGetResponse(cart.Id, cart.UserId, cart.CreatedAt, cart.UpdatedAt, cart.TotalPrice, cart.CartItems.Select(item => new CartItemGetResponse(item.Id, item.ProductId, item.Quantity, item.TotalPrice)).ToList());
    }
}