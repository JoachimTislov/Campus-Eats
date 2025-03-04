using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;
using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Microsoft.AspNetCore.Http;
using Mor_Qui_Sun_Tis_Lau.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Home;

public class IndexPage_RegisterTests
{
    private readonly Mock<IMediator> _mockMediator = new();
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<ILogger<TestableIndexModel>> _mockIndexLogger = new();
    private readonly Mock<IAdminService> _adminService = new();
    private readonly Mock<IProductRepository> _mockProductRepository = new();

    private readonly TestableIndexModel _indexPageModel;

    public IndexPage_RegisterTests()
    {
        _indexPageModel = new(_mockMediator.Object, _mockUserService.Object, _mockUserRepository.Object, _mockIndexLogger.Object, _adminService.Object, _mockProductRepository.Object)
        {
            RegisterViewModel = new()
            {
                Password = "Test@Password",
                Repeat_Password = "Test@Password",
                FirstName = "Test",
                LastName = "Tester"
            },
        };

        _indexPageModel.PageContext.HttpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task OnPostRegisterAsync_ShouldRegisterModelStateErrorAndReturnPage_WhenModelStateIsInvalid()
    {
        var errorName = "Test";
        var error = "Test ModelState Error";
        _indexPageModel.ModelState.AddModelError(errorName, error);

        var result = await _indexPageModel.OnPostRegisterAsync();

        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.False(_indexPageModel.ModelState.IsValid);

        Assert.True(_indexPageModel.ModelState.ContainsKey(errorName));
        Assert.Equal(error, _indexPageModel.ModelState[errorName]!.Errors[0].ErrorMessage);

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostRegisterAsync_ShouldAssignPasswordDoesNotMatchErrorAndReturnPage_WhenPasswordsDoNotMatch()
    {
        _indexPageModel.RegisterViewModel.Repeat_Password = "DifferentPasswordTest";

        var result = await _indexPageModel.OnPostRegisterAsync();

        Assert.Equal("Passwords do not match", _indexPageModel.RegisterViewModel.RegisterErrorMessage);
        Assert.IsAssignableFrom<IActionResult>(result);
    }

    [Fact]
    public async Task OnPostRegisterAsync_ShouldAssignUserCreationErrorAndReturnPage_WhenUserCreationDoesNotSucceed()
    {
        var errorDescription = "Creation of user failed";
        var errors = new IdentityError[]
        {
            new() { Description = errorDescription },
        };

        _mockUserService
            .Setup(sm => sm.Register(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<UserLoginInfo>()
            )).Returns(Task.FromResult((false, errors)));

        var result = await _indexPageModel.OnPostRegisterAsync();

        Assert.Equal(errors, _indexPageModel.RegisterViewModel.Errors);
        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostRegisterAsync_ShouldSignInUserAndRedirectToHomeIndex_WhenUserCreationSucceeds()
    {
        _mockUserService
            .Setup(sm => sm.Register(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<UserLoginInfo>()
            )).Returns(Task.FromResult((true, Array.Empty<IdentityError>())));

        var result = await _indexPageModel.OnPostRegisterAsync();

        var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
        Assert.Null(redirectResult.PageName);

        _mockIndexLogger.Verify(
        logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"{_indexPageModel.RegisterViewModel.Email} registered" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),

            Times.Once
        );
    }

}