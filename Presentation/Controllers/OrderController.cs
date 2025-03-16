using Application.DTOs.Orders;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _orderService.GetOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _orderService.GetOrderByIdAsync(userId, orderId);
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _orderService.CreateOrderAsync(userId, dto);
        return CreatedAtAction(nameof(GetOrder), new { orderId = order.Id }, order);
    }

    [HttpPut("{orderId}/status")]
    [Authorize(Roles = "Admin")] // Admin can change order status
    public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] string status)
    {
        await _orderService.UpdateOrderStatusAsync(orderId, status);
        return Ok($"Order status updated to {status}");
    }
}
