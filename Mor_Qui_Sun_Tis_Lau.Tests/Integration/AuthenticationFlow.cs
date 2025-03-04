using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Integration;

public class AuthenticationFlow(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    [Fact]
    public async Task Register_Logout_And_Login_For_User()
    {
        var client = _factory.CreateClient();

        client = await ConfiguredClientOperations.RegisterUser(client, "Edwin", "Petersen", "edwin.petersen@gmail.com");
        client = await ConfiguredClientOperations.LogoutUser(client);
        await ConfiguredClientOperations.LoginUser(client, "edwin.petersen@gmail.com");
    }

    [Fact]
    public async Task LoginAdmin_and_logout()
    {
        var client = _factory.CreateClient();

        await ConfiguredClientOperations.LoginAdmin(client);
        await ConfiguredClientOperations.LogoutUser(client);
    }

    /*
    Seeded users
        ("Edwin", "Larson", "edwinl@testmail.com", "98473212"),
        ("John", "Henry", "johnhenry@emailtest.net", "73832937"),
        ("Karl", "Uldman", "uldman@internationaltest.org", "93746592"),
        ("Martin", "Lauritsen", "mslrobo02@gmail.com", "93283782")
    */

    [Fact]
    public async Task LoginUser_AddProductToCart_CheckoutCart_PlaceOrder()
    {
        var client = _factory.CreateClient();

        var verificationToken = await VerificationTokenProvider.GetVerificationToken(client, "/");
        Assert.NotNull(verificationToken);

        client = await ConfiguredClientOperations.LoginUser(client, "edwinl@testmail.com");

        var content = new FormUrlEncodedContent(
        [
           new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
        ]);

        client = _factory.WithWebHostBuilder(builder =>
         {
             builder.ConfigureTestServices(services =>
             {
                 services.AddAuthentication(defaultScheme: "mock")
                     .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(
                         "mock", options => { });
             });
         }).CreateClient();

        var canteenResponse = await client.GetAsync("/Canteen");
        canteenResponse.EnsureSuccessStatusCode();
        var pageContent = await canteenResponse.Content.ReadAsStringAsync();

        var addToCartResponse = await client.PostAsync("/Canteen?handler=AddToCart", content);
        Assert.Equal(HttpStatusCode.OK, addToCartResponse.StatusCode);

        var checkoutCartResponse = await client.PostAsync("/Cart?handler=CheckoutCart", content);
        Assert.Equal(HttpStatusCode.OK, checkoutCartResponse.StatusCode);
    }
}