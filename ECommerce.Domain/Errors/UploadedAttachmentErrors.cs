using ECommerce.Domain.Common;

namespace ECommerce.Domain.Errors;

public static class UploadedAttachmentErrors
{
    public static readonly Error InvalidId =
        Error.Validation(
            "UploadedAttachment.InvalidId",
            "Attachment ID is required.");

    public static readonly Error InvalidPublicId =
        Error.Validation(
            "UploadedAttachment.InvalidPublicId",
            "Attachment Public ID is required.");

    public static readonly Error InvalidUrl =
        Error.Validation(
            "UploadedAttachment.InvalidUrl",
            "Attachment URL is required.");

    public static readonly Error AlreadyLinked =
        Error.Conflict(
            "UploadedAttachment.AlreadyLinked",
            "Attachment is already linked.");
}