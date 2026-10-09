using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.UploadedAttachments.Specifications;

public sealed class PendingAttachmentsSpecification: Specification<UploadedAttachment>
{
    public PendingAttachmentsSpecification()
    {
        Query.Where(a =>
            a.Status == AttachmentStatus.Pending);
    }
}