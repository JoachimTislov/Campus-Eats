using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.FulfillmentContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.InvoicingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.StripeContext.Services.Session;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Helpers;
using Mor_Qui_Sun_Tis_Lau.Pages.Customer.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Customer;

[Authorize(Roles = "Customer")]
public class OrderingPage(IMediator mediator, IFulfillmentService fulFillmentService, IInvoicingRepository invoicingRepository, IUserRepository userRepository, IOrderingService orderingService, IStripeSessionService stripeSessionService) : PageModel
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IFulfillmentService _fulFillmentService = fulFillmentService ?? throw new ArgumentNullException(nameof(fulFillmentService));
    private readonly IInvoicingRepository _invoicingRepository = invoicingRepository ?? throw new ArgumentNullException(nameof(invoicingRepository));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));
    private readonly IStripeSessionService _stripSessionService = stripeSessionService ?? throw new ArgumentNullException(nameof(stripeSessionService));


    [BindProperty(SupportsGet = true)]
    public Guid OrderId { get; set; }

    public Order? Order { get; set; }
    public Invoice? Invoice { get; set; }
    public Offer? Offer { get; set; }

    [BindProperty]
    public OrderingViewModel DeliveryViewModel { get; set; } = new();

    [BindProperty]
    public InvoiceAddressViewModel InvoiceAddressViewModel { get; set; } = new();

    private async Task<IActionResult> LoadOrder()
    {
        var customer = await _userRepository.GetUserByClaimsPrincipal(User);
        Order = await _orderingService.GetOrderById(OrderId);
        if (Order == null || customer == null || Order.CustomerId != customer.Id) return RedirectToPage(UrlProvider.Customer.Canteen);

        if (!Order.IsNew())
        {
            Invoice = await _invoicingRepository.GetInvoiceByOrderId(OrderId);
            Offer = await _fulFillmentService.GetOfferByOrderId(OrderId);
        }

        DeliveryViewModel = new(Order.CampusLocation, Order.Tip);
        InvoiceAddressViewModel = new(customer.Address);

        return Page();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        return await LoadOrder();
    }

    public async Task<IActionResult> OnPostPlaceOrderAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOrder();

            return Page();
        }

        await _orderingService.PlaceOrder(OrderId, new CampusLocation(DeliveryViewModel));
        await _mediator.Publish(new OrderPlaced(OrderId, new InvoiceAddress(InvoiceAddressViewModel)));

        return RedirectToPage(UrlProvider.Customer.Orders);
    }

    public async Task<IActionResult> OnGetHandleInvoicePaymentCallBackAsync(bool? transactionResult, string? invoiceId, string? sessionId)
    {
        if (transactionResult != null && transactionResult.Value && invoiceId != null && sessionId == UniqueCheckoutSessionId)
        {
            // Only handle when stripe redirects back to this page with given values
            await _invoicingRepository.HandleTransactionResult(transactionResult.Value, Guid.Parse(invoiceId));
        }

        return RedirectToPage();
    }

    private string GetBaseUrl() => $"{Request.Scheme}://{Request.Host}";

    // This is made to avoid users from bypassing payment, a unique string guid for every transaction
    private static string UniqueCheckoutSessionId { get; set; } = Guid.NewGuid().ToString();

    public async Task<IActionResult> OnPostPayInvoiceThroughStripeCheckoutSessionAsync(string invoiceId)
    {
        await LoadOrder();

        var sessionUrl = await _stripSessionService.PayInvoiceThroughACheckoutSession(GetBaseUrl(), Order!, invoiceId, UniqueCheckoutSessionId);

        Response.Headers.Append("Location", sessionUrl);
        return new StatusCodeResult(303);
    }

    public async Task<IActionResult> OnGetHandleTipCourierSessionCallBackAsync(bool? tipCourierTransactionResult, decimal? tip, string? sessionId)
    {
        if (tipCourierTransactionResult != null && tip != null && tipCourierTransactionResult.Value && sessionId == UniqueCheckoutSessionId)
        {
            await _orderingService.SetTipForOrder(OrderId, tip.Value);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTipCourierAsync(string orderStatus, int tip)
    {
        if (orderStatus != "Delivered") return new JsonResult(new { success = false, error = "Can't tip a order which isn't delivered :/" }); // Can't tip if the order isn't delivered
        if (tip < 10) return new JsonResult(new { success = false, error = "Can't tip less than 10 NOK" });
        if (tip > 150) return new JsonResult(new { success = false, error = "Can't tip more than 150 NOK" });

        var sessionUrl = await _stripSessionService.PayTipThroughACheckoutSession(GetBaseUrl(), OrderId, tip, UniqueCheckoutSessionId);
        return new JsonResult(new { success = true, url = sessionUrl });
    }
}