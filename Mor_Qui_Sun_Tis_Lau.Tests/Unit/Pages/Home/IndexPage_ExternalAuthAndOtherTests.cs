using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Home;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Home;

public class IndexPage_ExternalAuthAndOtherTests
{
    private readonly Mock<IMediator> _mockMediator = new();
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<ILogger<IndexModel>> _mockIndexLogger = new();
    private readonly IndexModel _indexModel;
    private readonly Mock<IAdminService> _adminService = new();
    private readonly Mock<IProductRepository> _mockProductRepository = new();

    public IndexPage_ExternalAuthAndOtherTests()
    {
        _indexModel = new(_mockMediator.Object, _mockUserService.Object, _mockUserRepository.Object, _mockIndexLogger.Object, _adminService.Object, _mockProductRepository.Object);

        _indexModel.PageContext.HttpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task OnGetAsync_UserIsAuthenticated_SetsPropertiesCorrectly()
    {
        // Arrange
        var authenticateType = "mock";

        _indexModel.PageContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new(ClaimTypes.NameIdentifier, "test-user-id"),
            new(ClaimTypes.Name, "test-user"),
            new(ClaimTypes.Email, "test@example.com")
        ], authenticateType));

        // Act
        await _indexModel.OnGetAsync();

        // Assert
        Assert.NotNull(_indexModel.User.Identity);
        Assert.True(_indexModel.User.Identity.IsAuthenticated);
        Assert.Equal(authenticateType, _indexModel.User.Identity.AuthenticationType);

        _mockUserRepository.Verify(m => m.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Once);
        _mockUserService.Verify(m => m.CheckIfUserIsAssignedRole(It.IsAny<ClaimsPrincipal>(), "Courier"), Times.Once);
        _mockUserService.Verify(m => m.CheckIfUserIsAssignedRole(It.IsAny<ClaimsPrincipal>(), "Admin"), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_ShouldNotDoAnything_WhenUserIsNotAuthenticated()
    {
        // Act
        await _indexModel.OnGetAsync();

        // Assert
        Assert.NotNull(_indexModel.User.Identity);
        Assert.False(_indexModel.User.Identity.IsAuthenticated);
        Assert.Null(_indexModel.User.Identity.AuthenticationType);

        _mockUserRepository.Verify(m => m.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Never);
        _mockUserService.Verify(m => m.CheckIfUserIsAssignedRole(It.IsAny<ClaimsPrincipal>(), "Courier"), Times.Never);
        _mockUserService.Verify(m => m.CheckIfUserIsAssignedRole(It.IsAny<ClaimsPrincipal>(), "Admin"), Times.Never);
    }

    private class ViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
    }

    /* Can't unit test it because TryValidateModel use services registered in the DI container
    [Fact]
    public void ClearModelStateAndValidateViewModel_ShouldClearModelStateAndValidateViewModel()
    {
        var key = "key";
        var description = "error description";
        _indexModel.ModelState.AddModelError(key, description);

        Assert.False(_indexModel.ModelState.IsValid);
        Assert.Equal(description, _indexModel.ModelState[key]?.Errors[0].ErrorMessage);

        var viewModel = new ViewModel();
        _indexModel.ClearModelStateAndValidateViewModel(viewModel);

        Assert.False(_indexModel.ModelState.IsValid);
        Assert.Equal("Name is required", _indexModel.ModelState["Name"]?.Errors[0].ErrorMessage);
    }*/

    [Fact]
    public async Task OnGetExternalLoginCallbackAsync_WithError_ShouldAddModelStateErrorAndRedirectToIndex()
    {
        // Act
        var result = await _indexModel.OnGetExternalLoginCallbackAsync(error: "some error");

        // Assert
        Assert.False(_indexModel.ModelState.IsValid);
        Assert.Equal("External login failed. Access was denied or an error occurred during login.", _indexModel.ModelState[string.Empty]?.Errors[0].ErrorMessage);

        var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
        Assert.Null(redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetExternalLoginCallbackAsync_LoginWithThirdPartySucceedsWithValidReturnUrl_RedirectsToReturnUrl()
    {
        // Arrange
        var returnUrl = "returnUrl";

        _mockUserService
            .Setup(sm => sm.LoginWithThirdParty(
                It.IsAny<HttpContext>()
            ))
            .ReturnsAsync(true);

        // Act
        var result = await _indexModel.OnGetExternalLoginCallbackAsync(returnUrl);

        // Assert
        var redirectResult = Assert.IsAssignableFrom<LocalRedirectResult>(result);
        Assert.Equal(returnUrl, redirectResult.Url);
    }

    [Fact]
    public async Task OnGetExternalLoginCallbackAsync_LoginWithThirdPartySucceeds_RedirectsToCanteen()
    {
        // Arrange
        _mockUserService
            .Setup(sm => sm.LoginWithThirdParty(
                It.IsAny<HttpContext>()
            ))
            .ReturnsAsync(true);

        // Act
        var result = await _indexModel.OnGetExternalLoginCallbackAsync();

        // Assert
        var redirectResult = Assert.IsAssignableFrom<LocalRedirectResult>(result);
        Assert.Equal("/Canteen", redirectResult.Url);
    }

    [Theory]
    [InlineData("Facebook")]
    [InlineData("Google")]
    public void OnGetExternalLoginAsync_ValidProvider_ReturnsChallengeResult(string provider)
    {

        // Act
        var result = _indexModel.OnGetExternalLoginAsync(provider);

        // Assert
        var challengeResult = Assert.IsType<ChallengeResult>(result);
        Assert.Equal(provider, challengeResult.AuthenticationSchemes[0]);
        Assert.NotNull(challengeResult.Properties);
        Assert.Equal("/?handler=ExternalLoginCallback", challengeResult.Properties.RedirectUri);
    }

    [Fact]
    public void OnGetExternalLoginAsync_InvalidProvider_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _indexModel.OnGetExternalLoginAsync("InvalidProvider"));
        Assert.Equal("Invalid provider (Parameter 'provider')", exception.Message);
    }

    [Fact]
    public async Task OnPostApplyForCourierAsync_ShouldNotPublishEvent_WhenShopIsNull()
    {
        // Act
        await _indexModel.OnPostApplyForCourierAsync("resume");

        // Assert
        _mockMediator.Verify(m => m.Publish(It.IsAny<UserAppliedForCourierRole>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task OnPostApplyForCourierAsync_ShouldPublishEvent()
    {
        _indexModel.PageContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity("mock"));

        User user = new();
        _mockUserRepository
            .Setup(m => m.GetUserByHttpContext(It.IsAny<HttpContext>()))
            .ReturnsAsync(user);

        // Act
        await _indexModel.OnPostApplyForCourierAsync("resume");

        // Assert
        _mockMediator.Verify(m => m.Publish(It.IsAny<UserAppliedForCourierRole>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}