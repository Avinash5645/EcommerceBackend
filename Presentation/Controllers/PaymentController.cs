using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-intent")]
    public async Task<IActionResult> CreatePaymentIntent(CreatePaymentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var clientSecret = await _paymentService.CreatePaymentIntentAsync(dto, userId);
        return Ok(new { clientSecret });
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-Signature"];

        try
        {
            await _paymentService.HandleWebhookAsync(json, stripeSignature!);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"Webhook error: {ex.Message}");
        }
    }
}
