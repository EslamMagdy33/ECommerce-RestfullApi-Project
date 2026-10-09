namespace ECommerce.Infrastructure.persistence.Seeding.Data.Models;

public record UploadedAttachmentSeedModel(
    Guid Id,
    string PublicId,
    string Url);