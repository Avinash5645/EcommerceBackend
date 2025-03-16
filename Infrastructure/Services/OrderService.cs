using Application.DTOs.Orders;
using Application.Interfaces;
using Core.Entities.Orders;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<Order> GetOrderByIdAsync(string userId, int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        return order ?? throw new Exception("Order not found");
    }

    public async Task<Order> CreateOrderAsync(string userId, CreateOrderDto dto)
    {
        var order = new Order
        {
            UserId = userId,
            Items = dto.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = _context.Products.Find(item.ProductId)?.Price ?? 0
            }).ToList(),
            TotalAmount = dto.Items.Sum(item =>
                _context.Products.Find(item.ProductId)?.Price ?? 0 * item.Quantity)
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId)
            ?? throw new Exception("Order not found");

        order.Status = status;
        await _context.SaveChangesAsync();
    }
}
