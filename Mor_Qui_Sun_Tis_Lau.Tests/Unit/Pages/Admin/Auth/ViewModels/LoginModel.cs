using Mor_Qui_Sun_Tis_Lau.Pages.Admin.Auth.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Admin.Auth.ViewModels;

public class LoginModelTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var loginViewModel = new LoginViewModel();

        Assert.Equal(string.Empty, loginViewModel.AdminName);
        Assert.Equal(string.Empty, loginViewModel.Password);
        Assert.Equal(string.Empty, loginViewModel.ErrorMessage);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var adminName = "AdminTest";
        var password = "TestPassword";
        var errorMessage = "ErrorMessage";

        var loginViewModel = new LoginViewModel()
        {
            AdminName = adminName,
            Password = password,
            ErrorMessage = errorMessage
        };

        Assert.Equal(adminName, loginViewModel.AdminName);
        Assert.Equal(password, loginViewModel.Password);
        Assert.Equal(errorMessage, loginViewModel.ErrorMessage);
    }
}