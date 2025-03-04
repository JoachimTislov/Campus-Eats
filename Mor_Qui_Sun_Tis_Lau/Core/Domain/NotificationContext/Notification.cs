using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext;

public class Notification(Guid userId, string title, string description, string? link) : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } = userId;
    public string Title { get; private set; } = title;
    public string Description { get; private set; } = description;
    public string? Link { get; private set; } = link;
}