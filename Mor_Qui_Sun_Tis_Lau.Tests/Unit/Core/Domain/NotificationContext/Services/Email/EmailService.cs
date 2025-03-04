using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.NotificationContext.Services.Email;

public class EmailServiceTests
{
    public EmailService _emailService = new();

    [Fact]
    public async Task SendEmailAsync_WhenPasswordIsMissing_ShouldReturnFalse()
    {
        var result = await _emailService.SendMailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

        Assert.False(result);
    }

    [Fact]
    public async Task SendEmailAsync_WhenPasswordIsWrong_ShouldReturnFailure()
    {
        Environment.SetEnvironmentVariable("AppPassword", "Wrong-Password");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<MailKit.Security.AuthenticationException>(async () =>
        {
            await _emailService.SendMailAsync("test@example.com", "Test Subject", "Test Body");
        });

        Assert.Contains("Username and Password not accepted", exception.Message);

        Environment.SetEnvironmentVariable("AppPassword", null);
    }
}