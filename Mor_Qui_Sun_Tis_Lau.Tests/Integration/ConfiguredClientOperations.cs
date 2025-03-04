
using System.Net;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Integration;

public class ConfiguredClientOperations
{
    public static async Task<HttpClient> RegisterUser(HttpClient client, string firstName, string lastName, string email)
    {
        var verificationToken = await VerificationTokenProvider.GetVerificationToken(client, "/");

        Assert.NotNull(verificationToken);

        var contentToSend = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("FirstName", firstName),
            new KeyValuePair<string, string>("LastName", lastName),
            new KeyValuePair<string, string>("Email", email),
            new KeyValuePair<string, string>("Password", "Test1234*"),
            new KeyValuePair<string, string>("Repeat_Password", "Test1234*"),
            new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
        ]);

        var register_response = await client.PostAsync("/?handler=Register", contentToSend);
        Assert.Equal(HttpStatusCode.OK, register_response.StatusCode);

        return client;
    }

    public static async Task<HttpClient> LoginUser(HttpClient client, string email)
    {
        var verificationToken = await VerificationTokenProvider.GetVerificationToken(client, "/");

        Assert.NotNull(verificationToken);

        var loginContent = new FormUrlEncodedContent(
       [
           new KeyValuePair<string, string>("Email", email),
            new KeyValuePair<string, string>("Password", "Test1234*"),
            new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
        ]);

        var login_response = await client.PostAsync("/?handler=Login", loginContent);
        Assert.Equal(HttpStatusCode.OK, login_response.StatusCode);

        return client;
    }

    public static async Task<HttpClient> LoginAdmin(HttpClient client)
    {
        var verificationToken = await VerificationTokenProvider.GetVerificationToken(client, "/Admin-login");

        Assert.NotNull(verificationToken);

        var loginContent = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("AdminName", "Admin"),
            new KeyValuePair<string, string>("Password", "Test1234*"),
            new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
        ]);

        var login_response = await client.PostAsync("/Admin-login?handler=Post", loginContent);
        Assert.Equal(HttpStatusCode.OK, login_response.StatusCode);

        return client;
    }

    public static async Task<HttpClient> LogoutUser(HttpClient client)
    {
        var verificationToken = await VerificationTokenProvider.GetVerificationToken(client, "/");
        Assert.NotNull(verificationToken);

        var logoutContent = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken),
        ]);

        var logout_response = await client.PostAsync("/Identity/Account/Logout?returnUrl=/", logoutContent);
        Assert.Equal(HttpStatusCode.OK, logout_response.StatusCode);

        return client;
    }
}