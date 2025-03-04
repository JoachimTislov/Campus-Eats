using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Integration;

public class ExternalAuthenticationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    [Fact]
    public async Task GoogleLogin_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var url = "/signin-google";

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task FacebookLogin_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var url = "/signin-facebook";

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}