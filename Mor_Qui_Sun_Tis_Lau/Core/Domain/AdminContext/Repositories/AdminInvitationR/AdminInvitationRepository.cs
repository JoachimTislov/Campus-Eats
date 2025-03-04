using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.AdminInvitationR;

public class AdminInvitationRepository(IMediator mediator, IDbRepository<AdminInvitation> dbRepository) : IAdminInvitationRepository
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IDbRepository<AdminInvitation> _dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));

    public async Task<AdminInvitation> SendAdminInvitation(Guid userId, string resume)
    {
        var adminInvitation = new AdminInvitation(userId, resume);

        await _dbRepository.AddAsync(adminInvitation);

        await _mediator.Publish(new AdminInvitationEvent(userId, adminInvitation.Id));

        return adminInvitation;
    }

    public async Task<AdminInvitation?> GetAdminInvitationByIdAsync(Guid invitationId)
    {
        return await _dbRepository.GetByIdAsync(invitationId);
    }
}