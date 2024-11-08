using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayPal.Api;
using System.Net.Http.Headers;
using System.Text;

namespace PaymentGateway.Services
{
    public class PayPalService
    {
        private readonly IConfiguration _configuration;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
        {
            { "mode", _configuration["PayPal:Mode"] }
        };

            var accessToken = new OAuthTokenCredential(
                _configuration["PayPal:ClientId"],
                _configuration["PayPal:Secret"]
            ).GetAccessToken();

            return new APIContext(accessToken) { Config = config };
        }

        public async Task<string> CreatePayment(string successUrl, string cancelUrl, decimal amount)
        {
            var apiContext = GetAPIContext();

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
            {
                new Transaction
                {
                    description = "Purchase from your site",
                    amount = new Amount { currency = "USD", total = amount.ToString("F") }
                }
            },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = cancelUrl,
                    return_url = successUrl
                }
            };

            var createdPayment = await Task.Run(() => payment.Create(apiContext));
            return createdPayment.GetApprovalUrl();
        }


        public async Task<string> GetAccessTokenAsync()
        {
            var client = new HttpClient();
            var clientId = _configuration["PayPal:ClientId"];
            var secret = _configuration["PayPal:Secret"];
            var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{secret}"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v1/oauth2/token")
            {
                Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var tokenResult = JsonConvert.DeserializeObject<dynamic>(result);
            return tokenResult.access_token;
        }




        public async Task<string> CreateSubscription(string planId)
        {
            var accessToken = await GetAccessTokenAsync();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var subscriptionData = new
            {
                plan_id = planId,
                application_context = new
                {
                    brand_name = "google.com",
                    return_url = "https://localhost:7008/Paypal/Success",
                    cancel_url = "https://localhost:7008/Paypal/Failure",
                    user_action = "SUBSCRIBE_NOW"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(subscriptionData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api-m.sandbox.paypal.com/v1/billing/subscriptions", content);

            var result = await response.Content.ReadAsStringAsync();
            var subscriptionResult = JsonConvert.DeserializeObject<dynamic>(result);

            var links = (IEnumerable<dynamic>)subscriptionResult.links;
            return links.FirstOrDefault(link => link.rel == "approve")?.href;
        }

        [HttpPost("CreateBillingPlan")]
        public async Task<string> CreateBillingPlanAsync(string jsonBody)
        {
            // Step 1: Access Token Generate karein
            var token = await GetAccessTokenAsync();

            // Step 2: Client setup aur request
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Step 3: PayPal API par POST request
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-m.sandbox.paypal.com/v1/billing/plans")
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            // Step 4: Response handle karein
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            // Step 5: Response return karein
            return result;
        }









    }

}
