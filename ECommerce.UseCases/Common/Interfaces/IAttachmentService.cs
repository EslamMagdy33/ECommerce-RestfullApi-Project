using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.DTOs;
using Microsoft.AspNetCore.Http;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IAttachmentService
{
    Task<Result<UploadedFile>> UploadAttachmentAsync(IFormFile file);
    Task<Result> DeleteAttachmentAsync(string publicId);
}
