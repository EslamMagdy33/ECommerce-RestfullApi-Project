namespace ECommerce.API.Models.Requests;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    IFormFile Image,
    Guid ProductBrandId,
    Guid ProductTypeId);
