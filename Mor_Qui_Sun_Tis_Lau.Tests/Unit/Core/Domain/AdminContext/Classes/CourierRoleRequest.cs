using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.AdminContext.Classes;

public class CourierRoleRequestTests
{
    [Fact]
    public void ConstructorWithValues_ShouldAssignValues()
    {
        var userId = Guid.NewGuid();
        var resume = "Test Resume";

        var courierRoleRequest = new CourierRoleRequest(userId, resume);

        Assert.Equal(Guid.Empty, courierRoleRequest.Id);
        Assert.Equal(userId, courierRoleRequest.UserId);
        Assert.Equal(resume, courierRoleRequest.Resume);
        Assert.Equal(CourierRoleRequestStatusEnum.Pending, courierRoleRequest.Status);

        Assert.False(courierRoleRequest.IsApproved());
    }

    [Fact]
    public void Status_ShouldAllowUpdates()
    {
        var courierRoleRequest = new CourierRoleRequest(Guid.NewGuid(), "Test Resume");

        courierRoleRequest.Declined();

        Assert.Equal(CourierRoleRequestStatusEnum.Declined, courierRoleRequest.Status);
        Assert.False(courierRoleRequest.IsApproved());

        courierRoleRequest.Approved();

        Assert.Equal(CourierRoleRequestStatusEnum.Approved, courierRoleRequest.Status);
        Assert.True(courierRoleRequest.IsApproved());
    }
}