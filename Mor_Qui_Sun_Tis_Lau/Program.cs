using DotNetEnv;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Hub;
using Stripe;
using Mor_Qui_Sun_Tis_Lau;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

Env.Load(); // loads sensitive variables
StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("StripeSecretKey") ?? throw new InvalidOperationException("Stripe secret key is missing in configuration");

ServicesBuilder.InitializeServices(services, builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();
var IsDevelopment = app.Environment.IsDevelopment();

// Configure the HTTP request pipeline.
if (!IsDevelopment)
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

using var scope = app.Services.CreateScope();
await DemoDataBuilder.BuildDemoData(scope.ServiceProvider, IsDevelopment);

app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();

public partial class Program { }
