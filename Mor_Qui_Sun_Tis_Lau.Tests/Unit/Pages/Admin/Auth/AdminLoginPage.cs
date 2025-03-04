using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.Auth;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Admin.Auth;

public class AdminLoginPageTests
{
    private readonly Mock<IAdminService> _mockAdminService = new();
    private readonly Mock<ILogger<AdminLoginModel>> _mockAdminLoginLogger = new();
    private readonly AdminLoginModel _adminLoginModel;
    private static string AdminName => "AdminName";

    public AdminLoginPageTests()
    {
        _adminLoginModel = new(_mockAdminService.Object, _mockAdminLoginLogger.Object)
        {
            ViewModel = new()
            {
                AdminName = AdminName
            }
        };
    }

    [Fact]
    public async Task OnPostAsync_ShouldAssignErrorMessage_WhenLoginSucceeds()
    {
        _mockAdminService
            .Setup(sm => sm.Login(
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync((true, It.IsAny<string>()));

        var result = await _adminLoginModel.OnPostAsync();

        var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Index, redirectResult.PageName);

        _mockAdminLoginLogger.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.Is<EventId>(eventId => eventId.Id == It.IsAny<int>()),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"Admin: {_adminLoginModel.ViewModel.AdminName} logged in" && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task OnPostAsync_ShouldAssignErrorMessage_WhenLoginFails()
    {
        var errorMessage = "login failed";
        _mockAdminService
            .Setup(sm => sm.Login(
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync((false, errorMessage));

        await _adminLoginModel.OnPostAsync();

        Assert.Equal(errorMessage, _adminLoginModel.ViewModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_ShouldAssignErrorMessage_WhenLoginFails_ErrorIsChangePassword_AndUsernameIsInvalid()
    {
        var errorMessage = "Change Password";
        _mockAdminService
            .Setup(sm => sm.Login(
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync((false, errorMessage));

        _mockAdminService
            .Setup(sm => sm.GeneratePasswordResetTokenAsync(
                It.IsAny<string>()
            ))
            .ReturnsAsync((false, It.IsAny<string>()));

        await _adminLoginModel.OnPostAsync();

        Assert.Equal("Invalid admin name", _adminLoginModel.ViewModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_ShouldRedirect_WhenLoginFailsAndErrorIsChangePassword()
    {
        var token = "token";

        var errorMessage = "Change Password";
        _mockAdminService
            .Setup(sm => sm.Login(
                It.IsAny<string>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync((false, errorMessage));

        _mockAdminService
            .Setup(sm => sm.GeneratePasswordResetTokenAsync(
                It.IsAny<string>()
            ))
            .ReturnsAsync((true, token));

        var result = await _adminLoginModel.OnPostAsync();

        var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.ForgotPassword, redirectResult.PageName);

        Assert.NotNull(redirectResult.RouteValues);

        Assert.Equal(token, redirectResult.RouteValues["Token"]);
        Assert.Equal(AdminName, redirectResult.RouteValues["EmailOrUsername"]);

        Assert.Equal(string.Empty, _adminLoginModel.ViewModel.ErrorMessage);
    }
}