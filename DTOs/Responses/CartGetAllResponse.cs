using Shahbazi.Store.Models;

namespace Shahbazi.Store.DTOs.Responses;

public record CartGetAllResponse(int Id, int UserId, DateTime CreatedAt, DateTime UpdatedAt, decimal TotalPrice);

public static class CartGetAllResponseExtensions
{
    public static CartGetAllResponse ToGetAllResponse(this Cart cart)
    {
        return new CartGetAllResponse(cart.Id, cart.UserId, cart.CreatedAt, cart.UpdatedAt, cart.TotalPrice);
    }
}