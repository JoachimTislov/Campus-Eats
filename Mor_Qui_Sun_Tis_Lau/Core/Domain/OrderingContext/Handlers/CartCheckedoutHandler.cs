using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Handlers;

public class CartCheckedOutHandler(IUserRepository userRepository, IOrderingRepository orderingRepository) : INotificationHandler<CartCheckedOut>
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IOrderingRepository _orderingRepository = orderingRepository ?? throw new ArgumentNullException(nameof(orderingRepository));

    public async Task Handle(CartCheckedOut notification, CancellationToken cancellationToken)
    {
        User? customer = await _userRepository.GetUserByClaimsPrincipal(notification.UserClaimsPrincipal);
        if (customer == null)
        {
            notification.OnOrderCreated.Invoke(Guid.Empty);
            return;
        }

        Order createdOrder = await _orderingRepository.CreateOrder(customer.Id);

        foreach (var cartItem in notification.CartItems)
        {
            createdOrder.AddOrderLine(cartItem.Name, cartItem.Price, cartItem.Count, cartItem.Stripe_productId);
        }

        await _orderingRepository.UpdateOrder(createdOrder);

        notification.OnOrderCreated.Invoke(createdOrder.Id);
    }
}