using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.persistence.Data.DbContexts;
using ECommerce.Infrastructure.persistence.Seeding;
using ECommerce.Infrastructure.persistence.Seeding.Data.Models;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public class ProductTypeSeeder(StoreDbContext dbContext) : IDataSeeder
{
    public int Order => 2;

    public async Task SeedAsync(CancellationToken ct = default)
        => await JsonSeeder.SeedIfEmpty<ProductType, ProductTypeSeedModel>
        (dbContext.Types, "types.json", t => ProductType.Create(t.Id, t.Name).Value, ct);

}
