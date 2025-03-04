
namespace Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

public static class VerificationTokenProvider
{
    public static async Task<string?> GetVerificationToken(HttpClient client, string url)
    {
        var defaultPage = await client.GetAsync(url);
        var verificationToken = await defaultPage.Content.ReadAsStringAsync();

        if (verificationToken != null && verificationToken.Length > 0)
        {
            verificationToken = verificationToken[verificationToken.IndexOf("__RequestVerificationToken")..];
            verificationToken = verificationToken[(verificationToken.IndexOf("value=\"") + 7)..];
            verificationToken = verificationToken[..verificationToken.IndexOf('\"')];
        }
        return verificationToken;
    }
}