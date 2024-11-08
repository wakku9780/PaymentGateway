using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Services;
using System.Threading.Tasks;

namespace PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaypalController : Controller
    {
        private readonly PayPalService _payPalService;

        public PaypalController(PayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpPost]
        public async Task<IActionResult> StartPayment(decimal amount)
        {
            var successUrl = Url.Action("Success", "Paypal", null, Request.Scheme);
            var cancelUrl = Url.Action("Failure", "Paypal", null, Request.Scheme);

            var redirectUrl = await _payPalService.CreatePayment(successUrl, cancelUrl, amount);
            return Redirect(redirectUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe()
        {
            var planId = "YOUR_TEST_PLAN_ID"; // Yahan par aap apna plan ID dalen
            var approvalUrl = await _payPalService.CreateSubscription(planId);

            // Redirect user to the PayPal approval URL
            return Redirect(approvalUrl);
        }


        [HttpPost("CreateBillingPlan")]
        public async Task<IActionResult> CreateBillingPlan()
        {
            // Step 1: JSON body prepare karein
            string jsonBody = @"
    {
        ""product_id"": ""YOUR_PRODUCT_ID"",
        ""name"": ""Basic Plan"",
        ""description"": ""Subscription plan for basic users"",
        ""status"": ""ACTIVE"",
        ""billing_cycles"": [
            {
                ""frequency"": {
                    ""interval_unit"": ""MONTH"",
                    ""interval_count"": 1
                },
                ""tenure_type"": ""REGULAR"",
                ""sequence"": 1,
                ""total_cycles"": 12,
                ""pricing_scheme"": {
                    ""fixed_price"": {
                        ""value"": ""10"",
                        ""currency_code"": ""USD""
                    }
                }
            }
        ],
        ""payment_preferences"": {
            ""auto_bill_outstanding"": true,
            ""setup_fee"": {
                ""value"": ""10"",
                ""currency_code"": ""USD""
            },
            ""setup_fee_failure_action"": ""CONTINUE"",
            ""payment_failure_threshold"": 3
        }
    }";

            // Step 2: `CreateBillingPlanAsync` method call karna
            var result = await _payPalService.CreateBillingPlanAsync(jsonBody);

            // Step 3: Response ko display karna
            return Ok(result);
        }




        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Success()
        {
            return View("Success");
        }

        public IActionResult Failure()
        {
            return View("Failure");
        }
    }
}