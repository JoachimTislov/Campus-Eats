
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.UserContext;

public class UserTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var user = new User();

        Assert.True(string.IsNullOrWhiteSpace(user.UserName));
        Assert.True(string.IsNullOrWhiteSpace(user.FirstName));
        Assert.True(string.IsNullOrWhiteSpace(user.LastName));
        Assert.Null(user.Email);
        Assert.Null(user.LastLoginDate);
        Assert.False(user.HasChangedPassword);
    }

    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        var firstName = "firstName";
        var lastName = "lastName";
        var email = "Test@Email";

        var user = new User(firstName, lastName, email);

        Assert.False(string.IsNullOrWhiteSpace(user.UserName));

        Assert.Equal(email, user.Email);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var firstName = "firstName";
        var lastName = "lastName";
        var email = "Test@Email";
        var lastLoginDate = new DateTime();

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            LastLoginDate = lastLoginDate,
            HasChangedPassword = true
        };

        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(email, user.Email);
        Assert.Equal(lastLoginDate, user.LastLoginDate);
        Assert.True(user.HasChangedPassword);
    }
}