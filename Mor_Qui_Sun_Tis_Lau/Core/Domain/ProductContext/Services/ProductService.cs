using Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Repository;
using Mor_Qui_Sun_Tis_Lau.Pages.Admin.ProductCRUD.ViewModels;

namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.ProductContext.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    private static string GetUploadImageFolder()
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        return uploadsFolder;
    }

    private static string GetFilePath(string fileName) => Path.Combine(GetUploadImageFolder(), fileName);

    private static async Task UploadImage(IFormFile imageFile, string uniqueFileName)
    {
        var filePath = GetFilePath(uniqueFileName);
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await imageFile.CopyToAsync(fileStream);
    }

    private async Task DeleteImgByProductId(Guid foodItemId)
    {
        var product = await GetFoodItemByIdAsync(foodItemId);
        if (product == null) return;

        // Avoid deleting the filler picture
        if (product.ImageLink != "filler.jpg")
        {
            var filePath = GetFilePath(product.ImageLink);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    private static async Task<string> HandleImageUploadAsync(IFormFile? imageFile, string existingImageLink)
    {
        if (imageFile == null) return existingImageLink;

        // If the uploaded file name matches the existing one, return the existing link
        if (existingImageLink != null && existingImageLink.Equals(imageFile.FileName, StringComparison.OrdinalIgnoreCase))
        {
            return existingImageLink;
        }

        // Save the new file
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
        await UploadImage(imageFile, uniqueFileName);
        return uniqueFileName; // Return the image link to save in the database
    }

    public (bool, string) ValidateImageFile(IFormFile imageFile)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return (false, "Invalid file format. Only JPG, PNG, and GIF are allowed.");
        }

        return (true, string.Empty);
    }

    public async Task CreateFoodItemAsync(FoodItemViewModel viewModel)
    {
        var imageLink = await HandleImageUploadAsync(viewModel.ImageFile, viewModel.ImageLink);
        await _productRepository.CreateFoodItem(viewModel.Name, viewModel.Description, viewModel.Price, imageLink);
    }

    public async Task EditFoodItemAsync(FoodItemViewModel viewModel)
    {
        // Deleting old picture if admin uploads a new one
        if (viewModel.ImageFile != null) await DeleteImgByProductId(viewModel.Id);

        var imageLink = await HandleImageUploadAsync(viewModel.ImageFile, viewModel.ImageLink);
        await _productRepository.EditFoodItem(viewModel.Id, viewModel.Name, viewModel.Description, viewModel.Price, imageLink);
    }

    public async Task DeleteFoodItemAsync(Guid productId)
    {
        await DeleteImgByProductId(productId);
        await _productRepository.DeleteFoodItem(productId);
    }

    public async Task<FoodItem?> GetFoodItemByIdAsync(Guid productId)
    {
        var product = await _productRepository.GetFoodItemById(productId);
        if (product == null) return null;

        return product;
    }
}
