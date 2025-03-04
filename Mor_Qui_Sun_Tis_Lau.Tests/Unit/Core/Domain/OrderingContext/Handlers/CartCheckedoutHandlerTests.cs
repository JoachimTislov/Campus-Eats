using System.Security.Claims;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.CartContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Handlers;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Core.Domain.OrderingContext.Handlers;

public class CartCheckedOutHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IOrderingRepository> _mockOrderingRepository = new();
    private readonly CartCheckedOutHandler _handler;

    public CartCheckedOutHandlerTests()
    {
        _handler = new CartCheckedOutHandler(_mockUserRepository.Object, _mockOrderingRepository.Object);
    }

    private static CartItem ReturnCartItem(Guid cartItemSku) => new(cartItemSku, "Test", 1m, 5, string.Empty, string.Empty);

    [Fact]
    public async Task Handle_ShouldCreateOrder()
    {
        // Arrange
        var user = new User();
        var order = new Order(user.Id);
        var itemId1 = Guid.NewGuid();
        var itemId2 = Guid.NewGuid();
        var cartItems = new List<CartItem>
        {
            ReturnCartItem(itemId1),
            ReturnCartItem(itemId2),
        };

        var onOrderCreatedCalled = false;

        var notification = new CartCheckedOut(
            cartItems, It.IsAny<ClaimsPrincipal>(),
            (orderId) =>
            {
                onOrderCreatedCalled = true;
                Assert.Equal(order.Id, orderId);
            }
        );

        _mockUserRepository
                .Setup(repo => repo.GetUserByClaimsPrincipal(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

        _mockOrderingRepository
            .Setup(repo => repo.CreateOrder(user.Id))
            .ReturnsAsync(order);

        _mockOrderingRepository
            .Setup(repo => repo.UpdateOrder(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        Assert.True(onOrderCreatedCalled);
        _mockOrderingRepository.Verify(repo => repo.CreateOrder(user.Id), Times.Once);
        _mockOrderingRepository.Verify(repo => repo.UpdateOrder(It.Is<Order>(o => o.Id == order.Id && o.OrderLines.Count() == 2)), Times.Once);
    }
}