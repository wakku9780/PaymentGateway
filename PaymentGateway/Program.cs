using PaymentGateway.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Stripe PaymentService
builder.Services.AddScoped<PaymentService>();


// Register PayPal Service
builder.Services.AddScoped<PayPalService>();

builder.Services.AddScoped<RazorpayService>();


// Configure Stripe settings
var stripeSettings = builder.Configuration.GetSection("Stripe");
StripeConfiguration.ApiKey = stripeSettings["SecretKey"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
