using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;

public class CourierRoleRequest(Guid userId, string resume) : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } = userId;
    public string Resume { get; private set; } = resume;
    public CourierRoleRequestStatusEnum Status { get; private set; } = CourierRoleRequestStatusEnum.Pending;

    public void Declined() => Status = CourierRoleRequestStatusEnum.Declined;
    public void Approved() => Status = CourierRoleRequestStatusEnum.Approved;

    public bool IsApproved() => Status == CourierRoleRequestStatusEnum.Approved;
}