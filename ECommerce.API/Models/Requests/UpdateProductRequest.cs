namespace ECommerce.API.Models.Requests;

public sealed record UpdateProductRequest(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IFormFile? Image,
    Guid ProductBrandId,
    Guid ProductTypeId);
