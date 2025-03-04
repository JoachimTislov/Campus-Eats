using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.AdminContext.Classes;

public class AdminTests
{
    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        var name = "adminTest";

        var admin = new Admin(name);

        Assert.NotNull(admin.Email);
        Assert.Equal(name, admin.UserName);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        // This looks weird, but the test passes :)
        var name = "adminTest";

        var admin = new Admin("adminTest2")
        {
            UserName = name
        };

        Assert.Equal(name, admin.UserName);
    }
}