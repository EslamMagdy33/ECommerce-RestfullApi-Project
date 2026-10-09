using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.persistence.Data.Configurations
{
    public sealed class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
        {
            BaseEntityConfiguration.Configure(builder);

            builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

            builder.HasIndex(t => t.Name)
                .IsUnique();
        }
    }
}
