using MyStripeProject.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentGateway.Services
{
    public class PaymentService
    {
        private readonly IConfiguration _configuration;

        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<string> CreateCheckoutSessionAsync(PaymentModel paymentModel, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = paymentModel.Currency,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = paymentModel.ProductName,
                    },
                    UnitAmount = paymentModel.Amount, // dynamic amount yahan use kar rahe hain
                },
                Quantity = paymentModel.Quantity,
            },
        },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Url;
        }





        public async Task<Charge> CreateChargeAsync(string customerEmail, long amountInCents, string currency)
        {
            var options = new ChargeCreateOptions
            {
                Amount = amountInCents,
                Currency = currency,
                ReceiptEmail = customerEmail,
                Source = "tok_visa", // In production, replace with a token from frontend
            };
            var service = new ChargeService();
            return await service.CreateAsync(options);
        }
    }
}
