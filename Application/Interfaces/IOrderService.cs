using Application.DTOs.Orders;
using Core.Entities.Orders;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetOrdersAsync(string userId);
        Task<Order> GetOrderByIdAsync(string userId, int orderId);
        Task<Order> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}
