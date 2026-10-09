namespace ECommerce.Infrastructure.persistence.Seeding.Data.Models;

public record ProductSeedModel(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid PictureAttachmentId,
    Guid TypeId,
    Guid BrandId);