using ECommerce.Domain.Common;

namespace ECommerce.Domain.Errors;

public static class AttachmentErrors
{
    public static readonly Error EmptyFile = Error.Validation("Attachment.EmptyFile", "File is empty.");
    public static readonly Error UploadFailed = Error.Failure("Attachment.UploadFailed", "Attachment upload failed.");
    public static readonly Error DeleteFailed = Error.Failure("Attachment.DeleteFailed", "Attachment deletion failed.");
    public static readonly Error CleanupFailed =Error.Failure("Attachment.CleanupFailed", "One or more orphan attachments could not be cleaned up.");
}
