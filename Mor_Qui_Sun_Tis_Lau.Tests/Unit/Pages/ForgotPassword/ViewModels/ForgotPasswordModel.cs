using Microsoft.AspNetCore.Identity;
using Mor_Qui_Sun_Tis_Lau.Pages.ForgotPassword.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.ForgotPassword.ViewModels;

public class ForgotPasswordModelTests
{
    [Fact]
    public void EmptyConstructor_ShouldHaveDefaultValues()
    {
        var forgotPasswordViewModel = new ForgotPasswordViewModel();

        Assert.Equal(forgotPasswordViewModel.Password, string.Empty);
        Assert.Equal(forgotPasswordViewModel.Repeat_Password, string.Empty);
        Assert.Equal(forgotPasswordViewModel.ForgotPasswordAlertMessage, string.Empty);
        Assert.Equal(forgotPasswordViewModel.Errors, []);
        Assert.False(forgotPasswordViewModel.AlertSuccess);
    }

    [Fact]
    public void Properties_ShouldAllowUpdates()
    {
        var password = "TestPassword";
        var repeat_password = "TestRepeatPassword";
        var alertMessage = "AlertMessage";
        IdentityError[] errors =
        [
            new IdentityError()
                {
                    Code = "Error",
                    Description = "ErrorDescription"
                }
        ];

        var forgotPasswordViewModel = new ForgotPasswordViewModel()
        {
            Password = password,
            Repeat_Password = repeat_password,
            ForgotPasswordAlertMessage = alertMessage,
            Errors = errors,
            AlertSuccess = true
        };

        Assert.Equal(password, forgotPasswordViewModel.Password);
        Assert.Equal(repeat_password, forgotPasswordViewModel.Repeat_Password);
        Assert.Equal(alertMessage, forgotPasswordViewModel.ForgotPasswordAlertMessage);
        Assert.Equal(errors, forgotPasswordViewModel.Errors);
        Assert.True(forgotPasswordViewModel.AlertSuccess);
    }
}