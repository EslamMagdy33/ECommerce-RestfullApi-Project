using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Products.Specifications;
using ECommerce.UseCases.UploadedAttachments.Specifications;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Services;

public sealed class AttachmentCleanupService
    (IRepository<UploadedAttachment> attachmentRepository, IRepository<Product> productRepository,
    IAttachmentService attachmentService,IUnitOfWork unitOfWork,ILogger<AttachmentCleanupService> logger)
    : IAttachmentCleanupService
{
    public async Task<Result> CleanupOrphanedAttachmentsAsync(CancellationToken ct = default)
    {
        var attachments = await attachmentRepository.ListIncludingDeletedAsync(new LinkedAttachmentsSpecification(), ct);

        var products = await productRepository.ListIncludingDeletedAsync(new AllProductsForAttachmentCleanupSpecification(), ct);

        var usedAttachmentIds = products.Select(p => p.PictureAttachmentId).ToHashSet();

        var orphanedAttachments = attachments.Where(a => !usedAttachmentIds.Contains(a.Id)).ToList();

        var hasFailures = false;

        foreach (var attachment in attachments)
        {
            if (usedAttachmentIds.Contains(attachment.Id))
                continue;
            logger.LogInformation("Orphan attachment found: {AttachmentId} - {PublicId}", attachment.Id, attachment.PublicId);

            var deleteResult = await attachmentService.DeleteAttachmentAsync(attachment.PublicId);

            if (deleteResult.IsFailure)
            {
                hasFailures = true;
                logger.LogError("Failed to delete orphan attachment from Cloudinary: {PublicId}", attachment.PublicId);
                continue;
            }

            attachmentRepository.Remove(attachment);
        }

        if (orphanedAttachments.Count > 0)
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        return hasFailures ? AttachmentErrors.CleanupFailed: Result.Success();
    }
}
