using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.ProductS;
using Stripe.Checkout;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.Session;

public class StripeSessionService(IStripeProductService stripeProductService) : IStripeSessionService
{
    private readonly IStripeProductService _stripeProductService = stripeProductService ?? throw new ArgumentNullException(nameof(stripeProductService));

    public async Task<string> PayInvoiceThroughACheckoutSession(string baseUrl, Order order, string invoiceId, string UniqueCheckoutSessionId)
    {
        string returnUrl(bool success) => $"{baseUrl}/Ordering/{order.Id}?handler=HandleInvoicePaymentCallBack&transactionResult={success}&invoiceId={invoiceId}&sessionId={UniqueCheckoutSessionId}";

        List<SessionLineItemOptions> lineItems = [];
        if (!order!.IsCanceled())
        {
            lineItems = await CreateSessionLineItemsOptions(order.OrderLines);
        }

        // This is creating a product for the delivery, which will be charged any time a user pays an invoice
        var productId = await _stripeProductService.CreateProduct("Delivery Fee", "This payment is required to cover the cost of the order being shipped to you", order.DeliveryFee, false, CancellationToken.None);
        lineItems.Add(await CreateSessionLineItemOptions(productId, 1));

        return await CreateSession(lineItems, returnUrl);
    }

    public async Task<string> PayTipThroughACheckoutSession(string baseUrl, Guid orderId, decimal tip, string UniqueCheckoutSessionId)
    {
        string returnUrl(bool success) => $"{baseUrl}/Ordering/{orderId}?handler=HandleTipCourierSessionCallBack&tipCourierTransactionResult={success}&tip={tip}&sessionId={UniqueCheckoutSessionId}";

        var productId = await _stripeProductService.CreateProduct("Tip", "80% of given amount wil be given to the courier", tip, false, CancellationToken.None);

        List<SessionLineItemOptions> lineItems = [await CreateSessionLineItemOptions(productId, 1)]; // Pretending the tip is a product so that we can use the session service

        return await CreateSession(lineItems, returnUrl);
    }

    private static async Task<string> CreateSession(List<SessionLineItemOptions> lineItems, Func<bool, string> returnUrl)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            LineItems = lineItems,
            SuccessUrl = returnUrl(true),
            CancelUrl = returnUrl(false),
        };

        return (await new SessionService().CreateAsync(options)).Url;
    }

    private async Task<List<SessionLineItemOptions>> CreateSessionLineItemsOptions(IEnumerable<OrderLine> orderLines)
    {
        List<SessionLineItemOptions> sessionLineItemOptions = [];

        foreach (var orderLine in orderLines)
        {
            sessionLineItemOptions.Add(await CreateSessionLineItemOptions(orderLine.Stripe_productId, orderLine.Quantity));
        }

        return sessionLineItemOptions;
    }

    private async Task<SessionLineItemOptions> CreateSessionLineItemOptions(string productId, int quantity)
    {
        var product = await _stripeProductService.GetProductById(productId);
        return new SessionLineItemOptions
        {
            Price = product.DefaultPriceId,
            Quantity = quantity
        };
    }
}

