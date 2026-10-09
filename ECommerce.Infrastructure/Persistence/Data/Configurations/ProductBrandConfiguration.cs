using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.persistence.Data.Configurations
{
    public sealed class ProductBrandConfiguration : IEntityTypeConfiguration<ProductBrand>
    {
        public void Configure(EntityTypeBuilder<ProductBrand> builder)
        {
            BaseEntityConfiguration.Configure(builder);
            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(b => b.Name)
                .IsUnique();

        }
    }
}
