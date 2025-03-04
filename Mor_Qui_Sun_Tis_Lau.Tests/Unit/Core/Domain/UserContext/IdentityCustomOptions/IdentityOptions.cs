using Microsoft.AspNetCore.Identity;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.IdentityCustomOptions;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.UserContext.IdentityCustomOptions;

public class IdentityOptionsTests
{
    [Fact]
    public void ConfigureIdentityOptions_ShouldAssignOptionsCorrectly()
    {
        var identityOptions = new IdentityOptions();

        IdentityOptionsConfiguration.ConfigureIdentityOptions(identityOptions);

        Assert.False(identityOptions.SignIn.RequireConfirmedEmail);
        Assert.False(identityOptions.SignIn.RequireConfirmedAccount);
        Assert.False(identityOptions.SignIn.RequireConfirmedPhoneNumber);

        Assert.Equal(TimeSpan.FromMinutes(2), identityOptions.Lockout.DefaultLockoutTimeSpan);
        Assert.Equal(3, identityOptions.Lockout.MaxFailedAccessAttempts);
        Assert.True(identityOptions.Lockout.AllowedForNewUsers);

        Assert.True(identityOptions.User.RequireUniqueEmail);
    }
}