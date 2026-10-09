namespace ECommerce.UseCases.Products.DTOs;

public record GetAllProductsResponse
    ( Guid Id, string Name, string Description, decimal Price,
    string PictureUrl, string ProductBrand, string ProductType );
