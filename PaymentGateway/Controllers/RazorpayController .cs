using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Services;

namespace PaymentGateway.Controllers
{
    
    public class RazorpayController : Controller
    {
        private readonly RazorpayService _razorpayService;

        public RazorpayController(RazorpayService razorpayService)
        {
            _razorpayService = razorpayService;
        }

        public IActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult StartPayment(decimal amount)
        {
            var order = _razorpayService.CreateOrder(amount);
            ViewBag.OrderId = order["id"].ToString();
            ViewBag.RazorpayKey = _razorpayService.KeyId;
            ViewBag.Amount = amount * 100;
            return View("Payment");
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
