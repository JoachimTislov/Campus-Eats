
namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Hub;

public interface INotificationClient
{
    Task NotifyClient(string message, string? link, string? nameOfLink);
    Task ReloadPage();
}