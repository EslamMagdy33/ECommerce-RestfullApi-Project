using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Errors;

namespace ECommerce.Domain.Entities;

public sealed class UploadedAttachment : BaseEntity
{
    public string PublicId { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public AttachmentStatus Status { get; private set; }

    private UploadedAttachment()
    {
    }

    private UploadedAttachment(
        Guid id,
        string publicId,
        string url,
        AttachmentStatus status)
    {
        Id = id;
        PublicId = publicId;
        Url = url;
        Status = status;
    }

    public static Result<UploadedAttachment> Create(
        Guid id,
        string publicId,
        string url)
    {
        if (id == Guid.Empty)
            return Result<UploadedAttachment>.Failure(
                UploadedAttachmentErrors.InvalidId);

        if (string.IsNullOrWhiteSpace(publicId))
            return Result<UploadedAttachment>.Failure(
                UploadedAttachmentErrors.InvalidPublicId);

        if (string.IsNullOrWhiteSpace(url))
            return Result<UploadedAttachment>.Failure(
                UploadedAttachmentErrors.InvalidUrl);

        return Result<UploadedAttachment>.Success(
            new UploadedAttachment(
                id,
                publicId.Trim(),
                url.Trim(),
                AttachmentStatus.Pending));
    }
    public static Result<UploadedAttachment> CreateLinked(
        Guid id,
        string publicId,
        string url)
    {
        if (id == Guid.Empty)
            return Result<UploadedAttachment>.Failure(
                UploadedAttachmentErrors.InvalidId);

        if (string.IsNullOrWhiteSpace(publicId))
            return Result<UploadedAttachment>.Failure(
                UploadedAttachmentErrors.InvalidPublicId);

        if (string.IsNullOrWhiteSpace(url))
            return Result<UploadedAttachment>.Failure(
                UploadedAttachmentErrors.InvalidUrl);

        return new UploadedAttachment(
            id,
            publicId.Trim(),
            url.Trim(),
            AttachmentStatus.Linked);
    }

    public Result MarkAsLinked()
    {
        if (Status == AttachmentStatus.Linked)
            return Result.Failure(
                UploadedAttachmentErrors.AlreadyLinked);

        Status = AttachmentStatus.Linked;

        return Result.Success();
    }
}
