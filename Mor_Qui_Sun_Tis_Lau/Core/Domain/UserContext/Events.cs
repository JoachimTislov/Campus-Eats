using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Enums;
using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

public record UserRegistered(Guid UserId, string FirstName, string UsersEmail, bool SendMail) : BaseDomainEvent;

public record UserAppliedForCourierRole(Guid UserId, string Resume) : BaseDomainEvent;
public record UserResponseToAdminInvitation(RequestActionEnum Action, Guid UserId, Guid InvitationId) : BaseDomainEvent;