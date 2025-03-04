using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;
using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;


namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Home;

public class IndexPage_LoginTests
{
    private readonly Mock<IMediator> _mockMediator = new();
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<ILogger<TestableIndexModel>> _mockIndexLogger = new();
    private readonly Mock<IAdminService> _adminService = new();
    private readonly Mock<IProductRepository> _mockProductRepository = new();

    private readonly TestableIndexModel _indexPageModel;

    public IndexPage_LoginTests()
    {
        _indexPageModel = new(_mockMediator.Object, _mockUserService.Object, _mockUserRepository.Object, _mockIndexLogger.Object, _adminService.Object, _mockProductRepository.Object)

        {
            LoginViewModel = new()
            {
                Email = "Test@Email",
                Password = "Test@Password"
            },
        };

        _indexPageModel.PageContext.HttpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task OnPostLoginAsync_ShouldRegisterModelStateErrorAndReturnPage_WhenModelStateIsInvalid()
    {
        var errorName = "Test";
        var error = "Test ModelState Error";
        _indexPageModel.ModelState.AddModelError(errorName, error);

        var result = await _indexPageModel.OnPostLoginAsync();

        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.False(_indexPageModel.ModelState.IsValid);

        Assert.True(_indexPageModel.ModelState.ContainsKey(errorName));
        Assert.Equal(error, _indexPageModel.ModelState[errorName]!.Errors[0].ErrorMessage);

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostLoginAsync_ShouldAssignErrorAndReturnPage_WhenLoginIsUnsuccessful()
    {
        var error = "Test login error";
        _mockUserService
            .Setup(sm => sm.Login(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .Returns(Task.FromResult((false, error)));

        var result = await _indexPageModel.OnPostLoginAsync();

        Assert.Equal(error, _indexPageModel.LoginViewModel.ErrorMessage);
        Assert.IsAssignableFrom<IActionResult>(result);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostLoginAsync_ShouldRedirectToIndexAndLogTheUserWhoLoggedIn_WhenLoginIsSuccessful()
    {
        _mockUserService
            .Setup(sm => sm.Login(
                It.IsAny<User>(),
                It.IsAny<string>()
            ))
            .Returns(Task.FromResult((true, string.Empty)));

        var result = await _indexPageModel.OnPostLoginAsync();

        var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Customer.Canteen, redirectResult.PageName);

        _mockIndexLogger.Verify(
        logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == $"{_indexPageModel.LoginViewModel.Email} logged in" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),

            Times.Once
        );
    }

}