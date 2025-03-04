using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Infrastructure;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        Environment.SetEnvironmentVariable("StripeSecretKey", "stripe-api-id");

        // Third Party
        Environment.SetEnvironmentVariable("GoogleClientId", "test-google-client-id");
        Environment.SetEnvironmentVariable("GoogleClientSecret", "test-google-client-secret");
        Environment.SetEnvironmentVariable("FacebookAppId", "test-facebook-app-id");
        Environment.SetEnvironmentVariable("FacebookAppSecret", "test-facebook-app-secret");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ShopContext>));

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddScoped(provider =>
            {
                var options = new DbContextOptionsBuilder<ShopContext>()
                    .UseSqlite(connection).Options;

                return new ShopContext(options);
            });

            var provider = services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
                db.Database.EnsureCreated();
            }

            // The following setup is required to test SignInManager, and by extension UserManager, as SignInManager depends on UserManager.
            var HttpContext = new DefaultHttpContext { RequestServices = provider };
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(c => c.HttpContext).Returns(HttpContext);
            services.AddSingleton(contextAccessor.Object);
        });
    }
}