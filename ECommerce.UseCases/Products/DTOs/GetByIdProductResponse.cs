namespace ECommerce.UseCases.Products.DTOs;

public record GetByIdProductResponse
    ( Guid Id, string Name, string Description, decimal Price, string PictureUrl,
        string ProductBrand, string ProductType);
