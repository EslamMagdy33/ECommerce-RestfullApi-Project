using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.DTOs;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Models;
using ECommerce.UseCases.Common.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services;

public class AttachmentService : IAttachmentService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<AttachmentService> _logger;

    public AttachmentService(IOptions<CloudinarySettings> config,ILogger<AttachmentService> logger)
    {
        _logger = logger;

        var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }
    public async Task<Result<UploadedFile>> UploadAttachmentAsync(FileUpload file)
    {
       if(file.Length <= 0)
        {
            _logger.LogWarning("Upload failed: file is empty");
            return Result<UploadedFile>.Failure(AttachmentErrors.EmptyFile);
        }
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "products",
            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
        };

        var result=await _cloudinary.UploadAsync(uploadParams);
        if(result.Error is not null)
        {
            _logger.LogError("Cloudinary upload failed: {Error}", result.Error.Message);
            return Result<UploadedFile>.Failure(AttachmentErrors.UploadFailed);
        }
        return Result<UploadedFile>.Success(new UploadedFile(result.SecureUrl.ToString(), result.PublicId));
    }

    public async Task<Result> DeleteAttachmentAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        if(result.Error is not null)
        {
            _logger.LogError("Cloudinary deletion failed For {PublicId}: {Error}", publicId, result.Error.Message);
            return Result.Failure(AttachmentErrors.DeleteFailed);
        }
        _logger.LogInformation("Attachment deleted successfully: {PublicId}", publicId);

        return Result.Success();
    }
}
