using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;

public record OrderStatusChanged(Guid OrderId, OrderStatusEnum Status, OrderStatusEnum? OldStatus = null) : BaseDomainEvent;
public record OrderPlaced(Guid OrderId, InvoiceAddress InvoiceAddressViewModel) : BaseDomainEvent;