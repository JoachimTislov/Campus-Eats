using Mor_Qui_Sun_Tis_Lau.Pages.Profile.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Profile.ViewModels;

public class ProfileModelTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var loginViewModel = new ProfileViewModel();

        Assert.Equal(string.Empty, loginViewModel.FirstName);
        Assert.Equal(string.Empty, loginViewModel.LastName);
        Assert.Equal(string.Empty, loginViewModel.Email);
        Assert.Equal(string.Empty, loginViewModel.PhoneNumber);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var firstName = "firstName";
        var lastName = "lastName";
        var email = "Test@Email.com";
        var phoneNumber = "1234567890";

        var profileViewModel = new ProfileViewModel()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };

        Assert.Equal(firstName, profileViewModel.FirstName);
        Assert.Equal(lastName, profileViewModel.LastName);
        Assert.Equal(email, profileViewModel.Email);
        Assert.Equal(phoneNumber, profileViewModel.PhoneNumber);
    }
}