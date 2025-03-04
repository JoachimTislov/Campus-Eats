using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext;
using System.ComponentModel.DataAnnotations;

namespace Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD.ViewModels;

public class FoodItemViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Image Link")]
    public string ImageLink { get; set; } = "filler.jpg";

    [Range(0.01, 2000.0, ErrorMessage = "Price must be greater than 0 and less than 2000 NOK")]
    [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Price must be a valid number with up to two decimal places.")]
    public decimal Price { get; set; }

    public string? Stripe_productId { get; set; }


    [Display(Name = "Upload Image")]
    public IFormFile? ImageFile { get; set; } // New property for file upload

    // Empty constructor for model binding
    public FoodItemViewModel()
    {
    }

    // Optional constructor to map from a FoodItem domain object
    public FoodItemViewModel(FoodItem foodItem)
    {
        Id = foodItem.Id;
        Name = foodItem.Name;
        Description = foodItem.Description;
        Price = foodItem.Price;
        ImageLink = foodItem.ImageLink;
        Stripe_productId = foodItem.Stripe_productId;
    }
}

