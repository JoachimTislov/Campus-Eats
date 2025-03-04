using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.AdminInvitationR;

public interface IAdminInvitationRepository
{
    Task<AdminInvitation> SendAdminInvitation(Guid userId, string Resume);
    Task<AdminInvitation?> GetAdminInvitationByIdAsync(Guid invitationId);
}