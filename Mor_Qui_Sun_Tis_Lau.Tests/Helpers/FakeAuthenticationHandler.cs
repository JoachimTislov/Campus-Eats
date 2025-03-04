using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

public class FakeAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Name, "test-user"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.GivenName, "Test"),
            new Claim(ClaimTypes.Surname, "User"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "Courier"),
            new Claim(ClaimTypes.Role, "Customer")
        };

        var identity = new ClaimsIdentity(claims, "mock");
        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, "mock");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    public static DefaultHttpContext CreateHttpContextForThirdPartyTesting(ClaimsPrincipal? user = null, string authenticationType = "mock")
    {
        var services = new ServiceCollection();
        services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(authenticationType, options => { });
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();

        user ??= new ClaimsPrincipal(new ClaimsIdentity(
            [
                new(ClaimTypes.Email, "test@Email.com")

            ], authenticationType));

        return new DefaultHttpContext
        {
            // Here to access the authentication scheme with given authentication type
            User = user,
            RequestServices = serviceProvider
        };
    }
}