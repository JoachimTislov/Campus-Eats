using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers.Data;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Integration.Routing;

public class UrlTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    private static string ReplaceValidGuidInUrl(string url)
    {
        if (url.Contains("{validGuid}"))
        {
            url = url.Replace("{validGuid}", Guid.NewGuid().ToString());
        }

        return url;
    }

    [Theory]
    [MemberData(nameof(UrlTestsData.UnAuthorizedUrls), MemberType = typeof(UrlTestsData))]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        url = ReplaceValidGuidInUrl(url);

        HttpClient client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        Assert.NotNull(response.Content.Headers.ContentType);

        Assert.Equal("text/html; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }

    [Theory]
    [MemberData(nameof(UrlTestsData.AuthorizedUrls), MemberType = typeof(UrlTestsData))]
    public async Task Get_TestIfAuthorizationRedirectsUnauthorizedUsers(string url)
    {
        // Arrange
        url = ReplaceValidGuidInUrl(url);

        HttpClient client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        var response = await client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("http://localhost/", response.Headers.Location?.OriginalString);
    }


    [Theory]
    [MemberData(nameof(UrlTestsData.AuthorizedUrls), MemberType = typeof(UrlTestsData))]
    public async Task Get_SecurePageIsReturnedForAnAuthenticatedUser(string url)
    {
        // Arrange
        url = ReplaceValidGuidInUrl(url);

        var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(defaultScheme: "mock")
                        .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(
                            "mock", options => { });
                });
            }).CreateClient();

        //Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}