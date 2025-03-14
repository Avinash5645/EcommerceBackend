using Application.DTOs.Carts;
using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId) ?? new Cart { UserId = userId };
        }

        public async Task AddToCartAsync(string userId, AddToCartDto dto)
        {
            var cart = await GetCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }

            if (cart.Id == 0)
            {
                _context.Carts.Add(cart);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(string userId, UpdateCartItemDto dto)
        {
            var cart = await GetCartAsync(userId);

            var item = cart.Items.FirstOrDefault(i => i.Id == dto.CartItemId);

            if (item == null)
            {
                throw new Exception("Item not found in cart");
            }

            item.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(string userId, int cartItemId)
        {
            var cart = await GetCartAsync(userId);

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item == null)
            {
                throw new Exception("Item not found in cart");
            }

            cart.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
