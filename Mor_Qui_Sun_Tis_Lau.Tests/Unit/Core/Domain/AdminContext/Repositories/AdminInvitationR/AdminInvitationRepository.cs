using MediatR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Repositories.AdminInvitationR;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.AdminContext.Repositories.AdminInvitationR;

public class AdminInvitationRepositoryTests
{
    private readonly Mock<IDbRepository<AdminInvitation>> _mockDbRepository = new();
    private readonly Mock<IMediator> _mockMediatR = new();

    private readonly AdminInvitationRepository _adminInvitationRepository;

    public AdminInvitationRepositoryTests()
    {
        _adminInvitationRepository = new(_mockMediatR.Object, _mockDbRepository.Object);
    }

    [Fact]
    public async Task SendAdminInvitation_And_GetAdminInvitationById_ShouldWorkCorrectly()
    {
        var userId = Guid.NewGuid();
        var resume = "resume";

        var adminInvitation = await _adminInvitationRepository.SendAdminInvitation(userId, resume);

        _mockDbRepository.Verify(m => m.AddAsync(adminInvitation), Times.Once);
        _mockMediatR.Verify(m => m.Publish(It.Is<AdminInvitationEvent>(a => a.UserId == userId && a.AdminInvitationId == adminInvitation.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAdminInvitationByIdAsync()
    {
        var adminInvitationId = Guid.NewGuid();

        await _adminInvitationRepository.GetAdminInvitationByIdAsync(adminInvitationId);

        _mockDbRepository.Verify(m => m.GetByIdAsync(adminInvitationId), Times.Once);
    }
}