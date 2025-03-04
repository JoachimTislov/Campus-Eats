using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;

public record AdminInvitationEvent(Guid UserId, Guid AdminInvitationId) : BaseDomainEvent;
public record UserAcceptedAdminInvitation(Guid UserId, string UsersEmail, string AdminName) : BaseDomainEvent;

public record CourierRoleRequestEvaluated(Guid UserId, bool Approved, string UsersEmail) : BaseDomainEvent;