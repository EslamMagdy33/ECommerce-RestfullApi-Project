using ECommerce.Domain.Common;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IAttachmentCleanupService
{
    Task<Result> CleanupOrphanedAttachmentsAsync(CancellationToken ct = default);
}