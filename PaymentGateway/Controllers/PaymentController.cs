using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyStripeProject.Models;
using PaymentGateway.Models;
using PaymentGateway.Services;

namespace PaymentGateway.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string email, int amount)
        {
            try
            {
                var charge = await _paymentService.CreateChargeAsync(email, amount, "usd");
                if (charge.Status == "succeeded")
                {
                    return RedirectToAction("Success");
                }
                return View("Failure");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StartPayment(int amount) // amount ko parameter ke roop me le rahe hain
        {
            // Convert dollar to cents for Stripe (e.g., $50.00 becomes 5000 cents)
            int amountInCents = amount * 100;

            var paymentModel = new PaymentModel
            {
                ProductName = "Sample Product",
                Amount = amountInCents, // Yaha dynamic amount assign ho raha hai
                Currency = "usd",
                Quantity = 1
            };

            var successUrl = Url.Action("Success", "Payment", null, Request.Scheme);
            var cancelUrl = Url.Action("Failure", "Payment", null, Request.Scheme);

            var checkoutUrl = await _paymentService.CreateCheckoutSessionAsync(paymentModel, successUrl, cancelUrl);
            return Redirect(checkoutUrl);
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Failure()
        {
            return View();
        }
    }
}
