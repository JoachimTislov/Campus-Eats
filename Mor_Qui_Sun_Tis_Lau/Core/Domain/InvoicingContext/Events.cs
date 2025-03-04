using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;

public record InvoicePaid(Guid OrderId) : BaseDomainEvent;