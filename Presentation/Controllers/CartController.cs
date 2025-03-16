using Application.DTOs.Carts;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[Authorize]  // Only authenticated users can access these endpoints
[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(AddToCartDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _cartService.AddToCartAsync(userId, dto);
        return Ok("Item added to cart");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateCartItem(UpdateCartItemDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _cartService.UpdateCartItemAsync(userId, dto);
        return Ok("Cart item updated");
    }

    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> RemoveCartItem(int cartItemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _cartService.RemoveCartItemAsync(userId, cartItemId);
        return Ok("Item removed from cart");
    }
}
