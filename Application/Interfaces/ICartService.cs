using Application.DTOs.Carts;
using Core.Entities;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(string userId);
        Task AddToCartAsync(string userId, AddToCartDto dto);
        Task UpdateCartItemAsync(string userId, UpdateCartItemDto dto);
        Task RemoveCartItemAsync(string userId, int cartItemId);
    }
}
