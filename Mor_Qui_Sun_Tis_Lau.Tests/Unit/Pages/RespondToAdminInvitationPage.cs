using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages;

public class RespondToAdminInvitationPageTests
{
    private readonly Mock<IAdminService> _mockAdminService = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();

    private readonly RespondToAdminInvitationModel _respondToAdminInvitationModel;

    public RespondToAdminInvitationPageTests()
    {
        _respondToAdminInvitationModel = new(_mockAdminService.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task OnGetAsync_ShouldAssignShopUserAndAdminInvitation_WhenTheyAreNotNull()
    {
        var adminInvitation = new AdminInvitation(Guid.NewGuid(), "resume");
        _mockAdminService
            .Setup(m => m.GetAdminInvitationByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(adminInvitation);

        var user = new User();
        _mockUserRepository
            .Setup(m => m.GetUserByHttpContext(It.IsAny<HttpContext>()))
            .ReturnsAsync(user);

        var result = await _respondToAdminInvitationModel.OnGetAsync();

        _mockAdminService.Verify(m => m.GetAdminInvitationByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockUserRepository.Verify(m => m.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Once);

        Assert.IsType<PageResult>(result);
        Assert.Equal(adminInvitation, _respondToAdminInvitationModel.AdminInvitation);
        Assert.Equal(user, _respondToAdminInvitationModel.ShopUser);
    }

    [Fact]
    public async Task OnGetAsync_ShouldRedirectToIndex_WhenAdminInvitationIsNull()
    {
        AdminInvitation? adminInvitation = null;
        _mockAdminService
            .Setup(m => m.GetAdminInvitationByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(adminInvitation);

        var user = new User();
        _mockUserRepository
            .Setup(m => m.GetUserByHttpContext(It.IsAny<HttpContext>()))
            .ReturnsAsync(user);

        var result = await _respondToAdminInvitationModel.OnGetAsync();

        _mockAdminService.Verify(m => m.GetAdminInvitationByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockUserRepository.Verify(m => m.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Once);

        var redirect = Assert.IsType<RedirectToPageResult>(result);

        Assert.Equal(UrlProvider.Index, redirect.PageName);

        Assert.Null(_respondToAdminInvitationModel.AdminInvitation);
        Assert.Equal(user, _respondToAdminInvitationModel.ShopUser);
    }

    [Fact]
    public async Task OnGetAsync_ShouldRedirectToIndex_WhenShopUserIsNull()
    {
        var adminInvitation = new AdminInvitation(Guid.NewGuid(), "resume");
        _mockAdminService
            .Setup(m => m.GetAdminInvitationByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(adminInvitation);

        User? user = null;
        _mockUserRepository
            .Setup(m => m.GetUserByHttpContext(It.IsAny<HttpContext>()))
            .ReturnsAsync(user);

        var result = await _respondToAdminInvitationModel.OnGetAsync();

        _mockAdminService.Verify(m => m.GetAdminInvitationByIdAsync(adminInvitation.Id), Times.Once);
        _mockUserRepository.Verify(m => m.GetUserByHttpContext(It.IsAny<HttpContext>()), Times.Once);

        var redirect = Assert.IsType<RedirectToPageResult>(result);

        Assert.Equal(UrlProvider.Index, redirect.PageName);

        Assert.Equal(adminInvitation, _respondToAdminInvitationModel.AdminInvitation);
        Assert.Null(_respondToAdminInvitationModel.ShopUser);
    }

    [Fact]
    public async Task OnPostRespondToInvitationAsync()
    {
        var result = await _respondToAdminInvitationModel.OnPostRespondToInvitationAsync(It.IsAny<RequestActionEnum>());

        var redirect = Assert.IsType<RedirectToPageResult>(result);

        Assert.Equal(UrlProvider.Index, redirect.PageName);

        _mockAdminService.Verify(m => m.RespondToInvitationAsync(It.IsAny<Guid>(), It.IsAny<RequestActionEnum>()), Times.Once);
    }
}