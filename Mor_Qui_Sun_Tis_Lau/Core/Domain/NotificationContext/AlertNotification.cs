using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext;

public class AlertNotification(Guid userId, string message, string? link, string? nameOfLink) : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } = userId;
    public string Message { get; private set; } = message;
    public string? Link { get; private set; } = link;
    public string? NameOfLink { get; private set; } = nameOfLink;
}