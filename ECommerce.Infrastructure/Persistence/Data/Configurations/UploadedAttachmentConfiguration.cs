using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.persistence.Data.Configurations;

internal sealed class UploadedAttachmentConfiguration
    : IEntityTypeConfiguration<UploadedAttachment>
{
    public void Configure(EntityTypeBuilder<UploadedAttachment> builder)
    {
        BaseEntityConfiguration.Configure(builder);

        builder.ToTable("UploadedAttachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.PublicId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(a => new
        {
            a.Status,
            a.CreatedAt
        });
    }
}
