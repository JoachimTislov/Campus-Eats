
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.NotificationContext;

public class NotificationTests
{
    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        var userId = Guid.NewGuid();
        var title = "title";
        var description = "description";
        var link = "link";

        var notification = new Notification(userId, title, description, link);

        Assert.Equal(Guid.Empty, notification.Id);
        Assert.Equal(userId, notification.UserId);
        Assert.Equal(title, notification.Title);
        Assert.Equal(description, notification.Description);
        Assert.Equal(link, notification.Link);
    }

    [Fact]
    public void Properties_AssertSetters()
    {
        var type = typeof(Notification);

        AssertSetter.AssertPrivate(type, nameof(Notification.Id));
        AssertSetter.AssertPrivate(type, nameof(Notification.UserId));
        AssertSetter.AssertPrivate(type, nameof(Notification.Title));
        AssertSetter.AssertPrivate(type, nameof(Notification.Description));
        AssertSetter.AssertPrivate(type, nameof(Notification.Link));
    }
}