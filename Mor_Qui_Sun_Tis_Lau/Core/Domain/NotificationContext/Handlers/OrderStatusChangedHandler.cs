using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.Email;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Services.NotificationS;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Services;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.NotificationContext.Handlers;

public class OrderStatusChangedHandler(IOrderingService orderingService, INotificationService notificationService, IUserRepository userRepository, IEmailService userService) : INotificationHandler<OrderStatusChanged>
{
    private readonly IOrderingService _orderingService = orderingService ?? throw new ArgumentNullException(nameof(orderingService));
    private readonly INotificationService _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IEmailService _emailService = userService ?? throw new ArgumentNullException(nameof(userService));

    public async Task Handle(OrderStatusChanged notification, CancellationToken cancellationToken)
    {
        var (orderId, status, _) = notification;

        var order = await _orderingService.GetOrderById(orderId);
        if (order == null) return;

        var customer = await _userRepository.GetUserById(order.CustomerId);
        if (customer == null) return;

        if (status == OrderStatusEnum.Placed)
        {
            await AlertEveryCourierThatAOrderWasPlaced();
        }
        else if (status == OrderStatusEnum.Picked || status == OrderStatusEnum.Shipped || status == OrderStatusEnum.Delivered)
        {
            await AlertRelatedMessageToOrderStatus(customer, order, status);
        }
    }

    private async Task AlertEveryCourierThatAOrderWasPlaced()
    {
        foreach (var courier in await _userRepository.GetUsersByRole("Courier"))
        {
            await _notificationService.CreateAlertNotification(courier.Id, "New order was placed!", "/Dashboard", "Go to dashboard");
            //await _notificationService.ReloadClientPage(courier.Id); Caused issues with redirection after placing order
        }
    }

    private async Task AlertRelatedMessageToOrderStatus(User customer, Order order, OrderStatusEnum orderStatus)
    {
        var linkToSpecificOrderPage = $"UrlProvider.Customer.Ordering/{order.Id}";

        string title = string.Empty;
        string description = string.Empty;

        switch (orderStatus)
        {
            case OrderStatusEnum.Picked:
                title = "Order Picked";
                description = "Your order has been picked up by a courier and will be shipped soon";
                break;

            case OrderStatusEnum.Shipped:
                title = "Order Shipped";
                description = "Your order has been picked up by a courier, please await further updates";
                break;

            case OrderStatusEnum.Delivered:
                title = "Order Delivered";
                description = "Your order has been delivered by a courier, enjoy your food!";
                break;
        }

        _ = _emailService.SendMailAsync(customer.Email!, title, description);
        await _notificationService.CreateNotification(order.CustomerId, title, description, linkToSpecificOrderPage);

        await _notificationService.CreateAlertNotification(customer.Id, description);
        await _notificationService.ReloadClientPage(customer.Id);
    }
}
