using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.DTOs;
using ECommerce.UseCases.Common.Models;
using Microsoft.AspNetCore.Http;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IAttachmentService
{
    Task<Result<UploadedFile>> UploadAttachmentAsync(FileUpload file);
    Task<Result> DeleteAttachmentAsync(string publicId);
}
