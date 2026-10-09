namespace ECommerce.UseCases.Products.DTOs;

public sealed record ProductForBasketResponse(Guid Id, string Name, string PictureUrl, decimal Price);