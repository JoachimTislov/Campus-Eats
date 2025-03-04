using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Services;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.AdminInvitation;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Admin.AdminInvitation;

public class AdminInvitationFormPageTests
{
    private readonly Mock<IAdminService> _mockAdminService = new();

    private readonly AdminInvitationFormModel _adminInvitationFormModel;

    private static readonly Guid UserId = new();
    private static readonly string Resume = "resume";

    public AdminInvitationFormPageTests()
    {
        _adminInvitationFormModel = new(_mockAdminService.Object)
        {
            UserId = UserId,
            Resume = Resume
        };
    }

    [Fact]
    public async Task OnPostAsync_ShouldReturnPage_WhenModelStateIsInvalid()
    {
        _adminInvitationFormModel.ModelState.AddModelError("key", "error");

        var result = await _adminInvitationFormModel.OnPostAsync();

        Assert.IsType<PageResult>(result);

        _mockAdminService.Verify(m => m.SendAdminInvitation(UserId, Resume), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_ShouldSendAdminInvitationAndRedirectToOverview_WhenModelStateIsValid()
    {
        var result = await _adminInvitationFormModel.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);

        Assert.Equal(UrlProvider.Admin.InvitationOverview, redirect.PageName);

        _mockAdminService.Verify(m => m.SendAdminInvitation(UserId, Resume), Times.Once);
    }
}