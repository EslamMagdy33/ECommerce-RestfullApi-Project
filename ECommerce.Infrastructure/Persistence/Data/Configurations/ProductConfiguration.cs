using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.persistence.Data.Configurations;

public sealed class ProductConfiguration
    : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        BaseEntityConfiguration.Configure(builder);

        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(Product.MaxNameLength);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(Product.MaxDescriptionLength);

        builder.Property(p => p.PictureAttachmentId)
            .IsRequired();

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.HasOne(p => p.ProductBrand)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.ProductBrandId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(p => p.ProductType)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.ProductTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(p => p.PictureAttachment)
            .WithMany()
            .HasForeignKey(p => p.PictureAttachmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(p => p.Name);

        builder.HasIndex(p => p.Price);
    }
}