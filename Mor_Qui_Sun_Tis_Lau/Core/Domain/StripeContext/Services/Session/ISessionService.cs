using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.Session;

public interface IStripeSessionService
{
    Task<string> PayInvoiceThroughACheckoutSession(string baseUrl, Order order, string invoiceId, string UniqueCheckoutSessionId);
    Task<string> PayTipThroughACheckoutSession(string baseUrl, Guid orderId, decimal tip, string UniqueCheckoutSessionId);
}

