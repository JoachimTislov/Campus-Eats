using Mor_Qui_Sun_Tis_Lau.Pages.Home.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Home.ViewModels;

public class LoginModelTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var loginViewModel = new LoginViewModel();

        Assert.Equal(string.Empty, loginViewModel.Email);
        Assert.Equal(string.Empty, loginViewModel.Password);
        Assert.Equal(string.Empty, loginViewModel.ErrorMessage);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var email = "Test@Email";
        var password = "TestPassword";
        var errorMessage = "ErrorMessage";

        var loginViewModel = new LoginViewModel()
        {
            Email = email,
            Password = password,
            ErrorMessage = errorMessage
        };

        Assert.Equal(email, loginViewModel.Email);
        Assert.Equal(password, loginViewModel.Password);
        Assert.Equal(errorMessage, loginViewModel.ErrorMessage);
    }
}