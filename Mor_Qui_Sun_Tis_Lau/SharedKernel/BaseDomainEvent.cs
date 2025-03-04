using MediatR;

namespace Mor_Qui_Sun_Tis_Lau.SharedKernel;

public abstract record BaseDomainEvent : INotification
{
    public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
}