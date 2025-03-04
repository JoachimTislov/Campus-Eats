
namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;

public interface IEmailService
{
    Task<bool> SendMailAsync(string email, string subject, string mailBody);
}