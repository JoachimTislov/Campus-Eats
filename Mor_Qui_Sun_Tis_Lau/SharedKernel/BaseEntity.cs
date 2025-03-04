namespace Mor_Qui_Sun_Tis_Lau.SharedKernel;

public abstract class BaseEntity
{
    public List<BaseDomainEvent> Events = [];
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
}