using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Classes;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.OrderingContext.Enum;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.UserContext.Repository;

namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;

public class OrderSeeder(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    public async Task SeedData(ShopContext db, IMediator mediator)
    {
        if (!db.Orders.Any())
        {
            var orderQuantities = new Dictionary<string, List<int>>
            {
                { "edwinl@testmail.com", new List<int> { 1, 1, 1, 2, 5, 2, 1, 2 } },
                { "johnhenry@emailtest.net", new List<int> { 1, 3, 2, 1, 1, 2, 1, 3 } },
                { "uldman@internationaltest.org", new List<int> { 1, 2, 1, 3, 2, 1, 1, 1 } }
            };

            var buildings = new List<string>() { "KE", "KA", "AR", "EOJ", "HG" };
            var roomNumbers = new List<string>() { "A-310", "E-102", "G-232", "B-203", "F-429" };
            var notes = new List<string>() { "Just call out my name when here", "Call me", "Center with blue shirt", "", "Back right" };
            var courier = await _userRepository.GetUserByEmail("mslrobo02@gmail.com");
            if (courier == null) return;

            var random = new Random();

            foreach (var orderQuantityObject in orderQuantities)
            {
                var customerEmail = orderQuantityObject.Key;
                var orderSpread = orderQuantityObject.Value;

                User? customer = await _userRepository.GetUserByEmail(customerEmail);
                if (customer == null) return;

                var count = 0;
                foreach (OrderStatusEnum status in Enum.GetValues(typeof(OrderStatusEnum)))
                {
                    for (int i = 0; i < orderSpread[count]; i++)
                    {
                        var newDate = DateTime.Today.AddMonths(-random.Next(0, 4));
                        newDate.AddYears(-random.Next(0, 2));
                        Order order = new(customer.Id) { OrderDate = newDate };
                        order.SetStatus(status);

                        if (status != OrderStatusEnum.New)
                        {
                            var building = buildings[random.Next(0, 5)];
                            var roomNumber = roomNumbers[random.Next(0, 5)];
                            var note = notes[random.Next(0, 5)];
                            var assignToUserMartin = random.Next(0, 6);
                            if (assignToUserMartin == 3) order.SetCourier(courier.Id);

                            var location = new CampusLocation(building, roomNumber, note);

                            order.CampusLocation = location;
                        }

                        var foodItemCount = db.FoodItems.Count() <= 3 ? 4 : db.FoodItems.Count();
                        int amountOfFoodItemsToAdd = random.Next(3, foodItemCount);

                        var foodItemsToAdd = db.FoodItems
                            .AsEnumerable()
                            .OrderBy(f => Guid.NewGuid())
                            .Take(amountOfFoodItemsToAdd)
                            .ToList();

                        foreach (var foodItem in foodItemsToAdd)
                        {
                            order.AddOrderLine(foodItem.Name, foodItem.Price, random.Next(1, 7), foodItem.Stripe_productId ?? string.Empty);
                        }

                        db.Orders.Add(order);
                        db.SaveChanges();

                        if (status == OrderStatusEnum.Placed)
                        {
                            // Invoice is seeded through the OrderStatusChanged event
                            await mediator.Publish(new OrderStatusChanged(order.Id, order.Status));
                        }
                    }
                    count += 1;
                }
            }
        }
    }
}