using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Infrastructure.persistence.Data.DbContexts;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductBrand> Brands=> Set<ProductBrand>();
    public DbSet<ProductType> Types=> Set<ProductType>();
    public DbSet<UserAddress> UserAddresses=> Set<UserAddress>();
    public DbSet<DeliveryMethod> DeliveryMethods => Set<DeliveryMethod>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<UploadedAttachment> UploadedAttachments => Set<UploadedAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly,
            type=>type.Namespace== "ECommerce.Infrastructure.persistence.Data.Configurations");

        base.OnModelCreating(modelBuilder);
    }
}
