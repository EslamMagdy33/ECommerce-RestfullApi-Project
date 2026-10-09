using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.UploadedAttachments.Specifications;

public sealed class LinkedAttachmentsSpecification:Specification<UploadedAttachment>
{
    public LinkedAttachmentsSpecification()
    {
        Query
            .Where(A => A.Status == AttachmentStatus.Linked);
    }
}
