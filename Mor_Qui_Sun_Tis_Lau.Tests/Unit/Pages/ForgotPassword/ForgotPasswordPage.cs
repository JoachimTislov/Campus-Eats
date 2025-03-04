using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Pages.ForgotPassword;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.ForgotPassword;

public class ForgotPasswordPageTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IEmailService> _mockEmailService = new();
    private readonly Mock<ILogger<ForgotPasswordModel>> _mockForgotPasswordLogger = new();
    private readonly ForgotPasswordModel _forgotPasswordModel;

    public ForgotPasswordPageTests()
    {
        _forgotPasswordModel = new(_mockUserRepository.Object, _mockUserService.Object, _mockEmailService.Object, _mockForgotPasswordLogger.Object)
        {
            ViewModel = new()
            {
                Password = "Test@Password",
                Repeat_Password = "Test@Password"
            }
        };
        _forgotPasswordModel.PageContext.HttpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task OnPostSendResetLinkAsync_ShouldAssignErrorMessageAndReturnPage_WhenGeneratePasswordResetTokenFails()
    {
        _mockUserService
            .Setup(sm => sm.GeneratePasswordResetTokenAsync(
                It.IsAny<User>()
            ))
            .ReturnsAsync((false, string.Empty));

        var result = await _forgotPasswordModel.OnPostSendResetLinkAsync();

        Assert.Equal("Invalid Email", _forgotPasswordModel.ViewModel.ForgotPasswordAlertMessage);
        Assert.False(_forgotPasswordModel.ViewModel.AlertSuccess);

        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSendResetLinkAsync_ShouldAssignErrorMessageAndReturnPage_WhenSendMailFails()
    {
        var token = "TestToken";
        _mockUserService
            .Setup(sm => sm.GeneratePasswordResetTokenAsync(
                It.IsAny<User>()
            ))
            .Returns(Task.FromResult((true, token)));

        _mockEmailService
            .Setup(sm => sm.SendMailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .Returns(Task.FromResult(false));

        var result = await _forgotPasswordModel.OnPostSendResetLinkAsync();

        Assert.Equal("Invalid Email", _forgotPasswordModel.ViewModel.ForgotPasswordAlertMessage);
        Assert.False(_forgotPasswordModel.ViewModel.AlertSuccess);

        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostSendResetLinkAsync_ShouldAssignSuccessMessageAndReturnPage_WhenSendMailSucceeds()
    {
        _forgotPasswordModel.EmailOrUsername = "Test@Email.com";

        _mockUserService
            .Setup(sm => sm.GeneratePasswordResetTokenAsync(
                It.IsAny<User>()
            ))
            .Returns(Task.FromResult((true, "TestToken")));

        _mockEmailService
            .Setup(sm => sm.SendMailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .Returns(Task.FromResult(true));

        var result = await _forgotPasswordModel.OnPostSendResetLinkAsync();

        Assert.Equal("Success! Check your email", _forgotPasswordModel.ViewModel.ForgotPasswordAlertMessage);
        Assert.True(_forgotPasswordModel.ViewModel.AlertSuccess);

        _mockForgotPasswordLogger.Verify(
        logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"Link to reset password for account with email: {_forgotPasswordModel.EmailOrUsername} sent successfully" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),

            Times.Once
        );

        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostChangePasswordAsync_ShouldRegisterModelStateErrorAndReturnPage_WhenModelStateIsInvalid()
    {
        var errorName = "Test";
        var error = "Test ModelState Error";
        _forgotPasswordModel.ModelState.AddModelError(errorName, error);

        var result = await _forgotPasswordModel.OnPostChangePasswordAsync("token", "email");

        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.False(_forgotPasswordModel.ModelState.IsValid);

        Assert.True(_forgotPasswordModel.ModelState.ContainsKey(errorName));
        Assert.Equal(error, _forgotPasswordModel.ModelState[errorName]!.Errors[0].ErrorMessage);

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostChangePasswordAsync_ShouldAssignPasswordDoesNotMatchErrorAndReturnPage_WhenPasswordsDoNotMatch()
    {
        _forgotPasswordModel.ViewModel.Repeat_Password = "DifferentPasswordTest";

        var result = await _forgotPasswordModel.OnPostChangePasswordAsync("token", "email");

        Assert.Equal("Passwords do not match", _forgotPasswordModel.ViewModel.ForgotPasswordAlertMessage);
        Assert.IsAssignableFrom<IActionResult>(result);
    }

    [Fact]
    public async Task OnPostChangePasswordAsync_ShouldAssignErrorMessage_WhenUserIsNotFound()
    {
        var token = "token";

        _mockUserService
            .Setup(sm => sm.ChangePassword(
                It.IsAny<User>(),
                Uri.EscapeDataString(token),
                _forgotPasswordModel.ViewModel.Password
            ))
            .Returns(Task.FromResult((false, true, Array.Empty<IdentityError>())));

        var result = await _forgotPasswordModel.OnPostChangePasswordAsync(token, It.IsAny<string>());

        Assert.Equal("Invalid change-password attempt", _forgotPasswordModel.ViewModel.ForgotPasswordAlertMessage);
        Assert.False(_forgotPasswordModel.ViewModel.AlertSuccess);

        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostChangePasswordAsync_ShouldAssignIdentityErrors_WhenSendMailFails()
    {
        var errorDescription = "Send mail failed";
        var errors = new IdentityError[]
        {
            new() { Description = errorDescription },
        };

        _mockUserService
            .Setup(sm => sm.ChangePassword(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .Returns(Task.FromResult((false, false, errors)));

        var result = await _forgotPasswordModel.OnPostChangePasswordAsync("TestToken", It.IsAny<string>());

        Assert.Equal(errors, _forgotPasswordModel.ViewModel.Errors);
        Assert.Equal(string.Empty, _forgotPasswordModel.ViewModel.ForgotPasswordAlertMessage);
        Assert.False(_forgotPasswordModel.ViewModel.AlertSuccess);

        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostChangePasswordAsync_ShouldAssignSuccessMessage_WhenSendMailSucceeds()
    {
        // Assigning Token to simulate a user accessing the page with a valid token
        var token = "TestToken";
        _forgotPasswordModel.Token = token;

        _mockUserService
            .Setup(sm => sm.ChangePassword(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .Returns(Task.FromResult((true, false, Array.Empty<IdentityError>())));

        var result = await _forgotPasswordModel.OnPostChangePasswordAsync(token, It.IsAny<string>());

        Assert.Equal("Successfully changed your password!", _forgotPasswordModel.ViewModel.ForgotPasswordAlertMessage);
        Assert.True(_forgotPasswordModel.ViewModel.AlertSuccess);

        Assert.Null(_forgotPasswordModel.Token);

        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.IsType<PageResult>(result);
    }
}