
using System.Security.Claims;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.UserContext;

public class EventsTests
{
    [Fact]
    public void UserRegisteredEvent()
    {
        var firstName = "firstName";
        var email = "Test@Email";
        var userId = Guid.NewGuid();
        var userRegisteredEvent = new UserRegistered(userId, firstName, email, true);

        Assert.Equal(firstName, userRegisteredEvent.FirstName);
        Assert.Equal(email, userRegisteredEvent.UsersEmail);
        Assert.True(userRegisteredEvent.SendMail);
    }

    [Fact]
    public void UserAppliedForCourierRole()
    {
        var userId = Guid.NewGuid();
        var resume = "resume";
        var userAppliedForCourierRole = new UserAppliedForCourierRole(userId, resume);

        Assert.Equal(userId, userAppliedForCourierRole.UserId);
        Assert.Equal(resume, userAppliedForCourierRole.Resume);
    }

    [Fact]
    public void UserResponseToAdminInvitation()
    {
        var action = RequestActionEnum.Accept;
        var userId = Guid.NewGuid();
        var adminInvitationId = Guid.NewGuid();

        var userResponseToAdminInvitation = new UserResponseToAdminInvitation(action, userId, adminInvitationId);

        Assert.Equal(userId, userResponseToAdminInvitation.UserId);
        Assert.Equal(action, userResponseToAdminInvitation.Action);
        Assert.Equal(adminInvitationId, userResponseToAdminInvitation.InvitationId);
    }
}