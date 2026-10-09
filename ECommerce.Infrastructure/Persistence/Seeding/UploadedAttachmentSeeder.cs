using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.persistence.Data.DbContexts;
using ECommerce.Infrastructure.persistence.Seeding;
using ECommerce.Infrastructure.persistence.Seeding.Data.Models;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public sealed class UploadedAttachmentSeeder(StoreDbContext dbContext)
    : IDataSeeder
{
    public int Order => 3;

    public async Task SeedAsync(CancellationToken ct = default)
        => await JsonSeeder.SeedIfEmpty<UploadedAttachment, UploadedAttachmentSeedModel>(
            dbContext.UploadedAttachments,"attachments.json",
            a => UploadedAttachment.CreateLinked(a.Id,a.PublicId,a.Url).Value,ct);
}