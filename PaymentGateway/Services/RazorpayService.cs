using Razorpay.Api;

namespace PaymentGateway.Services
{
    public class RazorpayService
    {
        private readonly string _keyId;
        private readonly string _keySecret;

        public RazorpayService(IConfiguration configuration)
        {
            _keyId = configuration["Razorpay:KeyId"];
            _keySecret = configuration["Razorpay:KeySecret"];
        }
        // Getter property for KeyId
        public string KeyId => _keyId;
        public Order CreateOrder(decimal amount)
        {
            var options = new Dictionary<string, object>
        {
            { "amount", amount * 100 },       // Amount in paisa (INR)
            { "currency", "INR" },
            { "payment_capture", 1 }
        };

            RazorpayClient client = new RazorpayClient(_keyId, _keySecret);
            Order order = client.Order.Create(options);
            return order;
        }
    }

}
