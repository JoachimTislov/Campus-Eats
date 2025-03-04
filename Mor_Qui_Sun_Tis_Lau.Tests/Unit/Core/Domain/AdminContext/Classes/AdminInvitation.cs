
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.AdminContext.Classes;

public class AdminInvitationTests
{
    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        var userId = Guid.NewGuid();
        var resume = "resume";

        var adminInvitation = new AdminInvitation(userId, resume);

        Assert.Equal(Guid.Empty, adminInvitation.Id);
        Assert.Equal(userId, adminInvitation.UserId);
        Assert.Equal(resume, adminInvitation.Resume);
        Assert.Equal(AdminInvitationStatusEnum.Pending, adminInvitation.Status);
    }

    [Fact]
    public void Status_ShouldAllowUpdates()
    {
        var adminInvitation = new AdminInvitation(Guid.NewGuid(), "resume");

        adminInvitation.Declined();

        Assert.Equal(AdminInvitationStatusEnum.Declined, adminInvitation.Status);

        adminInvitation.Approved();

        Assert.Equal(AdminInvitationStatusEnum.Approved, adminInvitation.Status);
    }
}