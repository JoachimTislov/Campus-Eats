using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Notifications;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Pages.Notifications;

public class NotificationsPageTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<INotificationService> _mockNotificationService = new();

    private readonly NotificationModel _notificationModel;

    public NotificationsPageTests()
    {
        _notificationModel = new(_mockUserRepository.Object, _mockNotificationService.Object);
    }

    [Fact]
    public async Task OnGetAsync_ShouldReturnToIndex_WhenUserIsNull()
    {
        User? user = null;
        _mockUserRepository
            .Setup(m => m.GetUserByClaimsPrincipal(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var result = await _notificationModel.OnGetAsync();

        _mockUserRepository.Verify(m => m.GetUserByClaimsPrincipal(It.IsAny<ClaimsPrincipal>()), Times.Once);
        _mockNotificationService.Verify(m => m.GetNotificationsFilteredByUserId(It.IsAny<Guid>()), Times.Never);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal(UrlProvider.Index, redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_ShouldGetNotificationsByUserIdAndAssignThem_WhenUserIsNotNull()
    {
        var user = new User();
        _mockUserRepository
            .Setup(m => m.GetUserByClaimsPrincipal(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        Notification notification = new(user.Id, "title", "description", "link");
        List<Notification> notifications = [notification];
        _mockNotificationService
            .Setup(m => m.GetNotificationsFilteredByUserId(user.Id))
            .ReturnsAsync(notifications);

        var result = await _notificationModel.OnGetAsync();

        _mockUserRepository.Verify(m => m.GetUserByClaimsPrincipal(It.IsAny<ClaimsPrincipal>()), Times.Once);
        _mockNotificationService.Verify(m => m.GetNotificationsFilteredByUserId(user.Id), Times.Once);

        Assert.IsType<PageResult>(result);

        Assert.NotEmpty(_notificationModel.Notifications);
        Assert.Equal(notification, _notificationModel.Notifications[0]);
    }
}
