using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.persistence.Data.DbContexts;
using ECommerce.Infrastructure.persistence.Seeding;
using ECommerce.Infrastructure.persistence.Seeding.Data.Models;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public sealed class ProductSeeder(StoreDbContext dbContext)
    : IDataSeeder
{
    public int Order => 4;

    public async Task SeedAsync(CancellationToken ct = default)
        => await JsonSeeder.SeedIfEmpty<Product, ProductSeedModel>(
            dbContext.Products,
            "products.json",
            p => Product.Create(
                p.Id,
                p.Name,
                p.Description,
                p.PictureAttachmentId,
                p.Price,
                p.BrandId,
                p.TypeId).Value,
            ct);
}