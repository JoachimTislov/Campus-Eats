using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.IdentityCustomOptions;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.UserContext.IdentityCustomOptions;
public class PasswordRequirementsTests
{
    [Fact]
    public void DefaultRequirements_ShouldBeValid()
    {
        var requirements = new PasswordRequirements();

        Assert.Equal(8, requirements.MinLength);
        Assert.True(requirements.RequireDigit);
        Assert.True(requirements.RequireLowercase);
        Assert.True(requirements.RequireNonAlphanumeric);
        Assert.True(requirements.RequireUppercase);
    }
}