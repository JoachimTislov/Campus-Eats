using Mor_Qui_Sun_Tis_Lau.SharedKernel;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;

public class FoodItem(string name, string description, decimal price, string? imageLink) : BaseEntity
{
    public Guid Id { get; private set; }
    private string _name = name;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                Events.Add(new FoodItemNameChanged(Id, _name, value));
                _name = value;
            }
        }
    }
    public string Description { get; set; } = description;
    public string ImageLink { get; set; } = GetValidImageLink(imageLink);
    private decimal _price = price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                Events.Add(new FoodItemPriceChanged(Id, _price, value));
                _price = value;
            }
        }
    }

    public string? Stripe_productId { get; set; }

    private static string GetValidImageLink(string? imageLink) => string.IsNullOrWhiteSpace(imageLink) ? "filler.jpg" : imageLink;

    public void EditFoodItem(string name, string description, decimal price, string? imageLink)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageLink = GetValidImageLink(imageLink);
    }
}