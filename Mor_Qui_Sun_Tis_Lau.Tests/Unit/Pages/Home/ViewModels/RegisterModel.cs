using Microsoft.AspNetCore.Identity;
using Mor_Qui_Sun_Tis_Lau.Pages.Home.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Home.ViewModels;

public class RegisterModelTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var registerViewModel = new RegisterViewModel();

        Assert.Equal(string.Empty, registerViewModel.Email);
        Assert.Equal(string.Empty, registerViewModel.Password);
        Assert.Equal(string.Empty, registerViewModel.Repeat_Password);
        Assert.Equal(string.Empty, registerViewModel.RegisterErrorMessage);
        Assert.Empty(registerViewModel.Errors);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var email = "TestEmail";
        var password = "TestPassword";
        var repeat_password = "TestRepeatPassword";
        var registerErrorMessage = "ErrorMessage";

        IdentityError[] errors =
        [
            new IdentityError()
            {
                Code = "Error",
                Description = "ErrorDescription"
            }
        ];

        var registerViewModel = new RegisterViewModel()
        {
            Email = email,
            Password = password,
            Repeat_Password = repeat_password,
            RegisterErrorMessage = registerErrorMessage,
            Errors = errors
        };

        Assert.Equal(email, registerViewModel.Email);
        Assert.Equal(password, registerViewModel.Password);
        Assert.Equal(repeat_password, registerViewModel.Repeat_Password);
        Assert.Equal(registerErrorMessage, registerViewModel.RegisterErrorMessage);
        Assert.Equal(errors, registerViewModel.Errors);
    }
}