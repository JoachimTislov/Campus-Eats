using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.NotificationContext.Handlers;

public class UserRegisteredHandlerTests
{
    private readonly Mock<IEmailService> _mockEmailService = new();
    private readonly Mock<INotificationService> _mockNotificationService = new();

    private readonly UserRegisteredHandler _userRegisteredHandler;

    public UserRegisteredHandlerTests()
    {
        _userRegisteredHandler = new(_mockEmailService.Object, _mockNotificationService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturn_WhenSendMailIsFalse()
    {
        var userRegistered = new UserRegistered(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), false);

        await _userRegisteredHandler.Handle(userRegistered, CancellationToken.None);

        _mockEmailService.Verify(m => m.SendMailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSendMail_WhenSendMailIsTrue()
    {
        var usersEmail = "users@Email.com";
        var userRegistered = new UserRegistered(It.IsAny<Guid>(), It.IsAny<string>(), usersEmail, true);

        await _userRegisteredHandler.Handle(userRegistered, CancellationToken.None);

        _mockEmailService
            .Verify(m => m.SendMailAsync(
                usersEmail,
                "Welcome to campus eats!",
                "Hey, \n Welcome to campus eats! \n\n -- Campus Eats"
            ), Times.Once);
    }
}