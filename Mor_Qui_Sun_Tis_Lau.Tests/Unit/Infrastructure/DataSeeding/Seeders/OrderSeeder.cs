using MediatR;
using Moq;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Infrastructure;
using Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;
using Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

namespace Mor_Qui_Sun_Tis_Lau.Tests.Unit.Infrastructure.DataSeeding.Seeders;

public class OrderSeederTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IMediator> _mockMediator = new();
    private readonly ShopContext _shopContext;
    private readonly OrderSeeder _orderSeeder;

    public OrderSeederTests()
    {
        _orderSeeder = new(_mockUserRepository.Object);
        _shopContext = DbTest.CreateContext();
    }

    [Fact]
    public async Task SeedData_ShouldSeedOrders_WhenTheyDoNotExist()
    {
        _shopContext.Orders.RemoveRange(_shopContext.Orders);
        await _shopContext.SaveChangesAsync();

        var userEdwin = new User { Email = "edwinl@testmail.com" };
        var userJohn = new User { Email = "johnhenry@emailtest.net" };
        var userUldman = new User { Email = "uldman@internationaltest.org" };
        var userMsl = new User { Email = "mslrobo02@gmail.com" };

        _mockUserRepository
            .Setup(u => u.GetUserByEmail(userEdwin.Email))
            .ReturnsAsync(userEdwin);

        _mockUserRepository
            .Setup(u => u.GetUserByEmail(userJohn.Email))
            .ReturnsAsync(userJohn);

        _mockUserRepository
            .Setup(u => u.GetUserByEmail(userUldman.Email))
            .ReturnsAsync(userUldman);

        _mockUserRepository
            .Setup(u => u.GetUserByEmail(userMsl.Email))
            .ReturnsAsync(userMsl);

        await _orderSeeder.SeedData(_shopContext, _mockMediator.Object);

        _mockUserRepository.Verify(m => m.GetUserByEmail(It.IsAny<string>()), Times.Exactly(4));
        _mockMediator.Verify(m => m.Publish(It.IsAny<OrderStatusChanged>(), It.IsAny<CancellationToken>()), Times.Exactly(6));
        Assert.True(_shopContext.Orders.Any());
        Assert.Equal(35, _shopContext.Orders.Count());
    }

    [Fact]
    public async Task SeedData_ShouldNotSeedOrders_WhenUserDoesNotExist()
    {
        _shopContext.Orders.RemoveRange(_shopContext.Orders);
        await _shopContext.SaveChangesAsync();

        await _orderSeeder.SeedData(_shopContext, _mockMediator.Object);

        _mockUserRepository.Verify(m => m.GetUserByEmail(It.IsAny<string>()), Times.Once);
        _mockMediator.Verify(m => m.Publish(It.IsAny<OrderStatusChanged>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.False(_shopContext.Orders.Any());
    }

    [Fact]
    public async Task SeedData_ShouldNotSeedOrders_WhenOrdersAlreadyExist()
    {
        _shopContext.Orders.RemoveRange(_shopContext.Orders);
        await _shopContext.SaveChangesAsync();

        var userEdwin = new User { Email = "edwinl@testmail.com" };
        var userJohn = new User { Email = "edwinl@testmail.com" };
        var userUldman = new User { Email = "edwinl@testmail.com" };

        _shopContext.Orders.Add(new Order(userEdwin.Id));
        await _shopContext.SaveChangesAsync();

        _mockUserRepository
            .Setup(u => u.GetUserByEmail("edwinl@testmail.com"))
            .ReturnsAsync(userEdwin);

        _mockUserRepository
            .Setup(u => u.GetUserByEmail("johnhenry@emailtest.net"))
            .ReturnsAsync(userJohn);

        _mockUserRepository
            .Setup(u => u.GetUserByEmail("uldman@internationaltest.org"))
            .ReturnsAsync(userUldman);

        await _orderSeeder.SeedData(_shopContext, _mockMediator.Object);

        _mockUserRepository.Verify(m => m.GetUserByEmail(It.IsAny<string>()), Times.Never);
        _mockMediator.Verify(m => m.Publish(It.IsAny<OrderStatusChanged>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.True(_shopContext.Orders.Any());
    }
}