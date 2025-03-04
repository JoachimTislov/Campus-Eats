using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;

public class AdminInvitation(Guid userId, string resume) : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } = userId;
    public string? Resume { get; private set; } = resume;
    public AdminInvitationStatusEnum Status { get; private set; } = AdminInvitationStatusEnum.Pending;

    public void Declined() => Status = AdminInvitationStatusEnum.Declined;
    public void Approved() => Status = AdminInvitationStatusEnum.Approved;
}