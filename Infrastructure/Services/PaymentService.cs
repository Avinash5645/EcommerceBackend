using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Configurations;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly StripeSettings _stripeSettings;

    public PaymentService(AppDbContext context, IOptions<StripeSettings> stripeSettings)
    {
        _context = context;
        _stripeSettings = stripeSettings.Value;
    }

    public async Task<string> CreatePaymentIntentAsync(CreatePaymentDto dto, string userId)
    {
        var order = await _context.Orders.FindAsync(dto.OrderId)
            ?? throw new Exception("Order not found");

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(order.TotalAmount * 100), // Stripe expects amount in cents
            Currency = "usd",
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", order.Id.ToString() },
                { "UserId", userId }
            }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return paymentIntent.ClientSecret;
    }

    public async Task HandleWebhookAsync(string json, string stripeSignature)
    {
        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                _stripeSettings.WebhookSecret
            );
        }
        catch (Exception ex)
        {
            throw new Exception("Webhook error: " + ex.Message);
        }

        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            var orderId = int.Parse(paymentIntent.Metadata["OrderId"]);
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null)
            {
                order.Status = "Completed";
                await _context.SaveChangesAsync();
            }
        }
        else if (stripeEvent.Type == "payment_intent.payment_failed")
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            var orderId = int.Parse(paymentIntent.Metadata["OrderId"]);
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null)
            {
                order.Status = "Failed";
                await _context.SaveChangesAsync();
            }
        }
    }

}
