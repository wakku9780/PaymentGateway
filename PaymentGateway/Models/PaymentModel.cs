namespace MyStripeProject.Models
{
    public class PaymentModel
    {
        public string ProductName { get; set; } = "Sample Product"; // Default product
        public int Amount { get; set; } = 5000; // Amount in cents (e.g., $50.00)
        public string Currency { get; set; } = "usd"; // Default currency
        public int Quantity { get; set; } = 1; // Default quantity
    }
}
