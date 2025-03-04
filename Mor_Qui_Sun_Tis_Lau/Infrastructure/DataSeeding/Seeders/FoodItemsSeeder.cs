using MediatR;
using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;

namespace Mor_Qui_Sun_Tis_Lau.Infrastructure.DataSeeding.Seeders;

public static class FoodItemsSeeder
{
    public static async Task SeedData(ShopContext db, IMediator mediator, bool IsDevelopment)
    {
        if (!db.FoodItems.Any())
        {
            var itemNames = new[] { "Tender Beef", "Cheese Pizza", "Burger’n Fries", "Chicken Tikka Masala", "Campus Sandwich", "Vegan Burger", "Beefy Campus Wraps", "Berry Booster", "Taco Friday", "Coca Cola", "Classic Smoothie", "Salad" };
            var prices = new decimal[] { 150.00m, 120.00m, 200.00m, 150.00m, 85.00m, 135.00m, 110.00m, 50.00m, 120.00m, 20.00m, 35.00m, 45.00m };

            var description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Assumenda, odit totam. Ratione mollitia possimus ducimus nesciunt nisi eos architecto eveniet corrupti. Saepe nostrum amet eligendi ratione laudantium, beatae ea maxime!";
            var imgLink = string.Empty;

            for (int i = 0; i < itemNames.Length; i++)
            {
                var foodItem = new FoodItem(itemNames[i], description, prices[i], imgLink);
                db.FoodItems.Add(foodItem);
                db.SaveChanges();

                if (IsDevelopment)
                {
                    await mediator.Publish(new ProductCreated(foodItem.Id, foodItem.Name, foodItem.Description, foodItem.Price));
                }
            }
        }
    }
}