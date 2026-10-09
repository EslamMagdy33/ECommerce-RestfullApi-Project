using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandle<CreateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IAttachmentService attachmentService;
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<UploadedAttachment> attachmentRepository;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        IAttachmentService attachmentService,
        IRepository<Product> productRepository,
        IRepository<UploadedAttachment> attachmentRepository)
    {
        this.unitOfWork = unitOfWork;
        this.attachmentService = attachmentService;
        this.productRepository = productRepository;
        this.attachmentRepository = attachmentRepository;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken ct = default)
    {
        // 1. Check duplicate product name
        var existingProduct =
            await productRepository.AnyAsync(
                new ProductByNameSpecification(request.Name),
                ct);

        if (existingProduct)
        {
            return Result<Guid>.Failure(ProductErrors.DuplicateName);
        }

        // 2. Upload image to Cloudinary
        var imageResult =
            await attachmentService.UploadAttachmentAsync(request.Image);

        if (imageResult.IsFailure)
            return Result<Guid>.Failure(imageResult.Error!);

        var uploadedFile = imageResult.Value;

        // 3. Create Attachment entity
        var attachmentId = Guid.NewGuid();

        var attachmentResult =
            UploadedAttachment.Create(
                attachmentId,
                uploadedFile.PublicId,
                uploadedFile.Url);

        if (attachmentResult.IsFailure)
        {
            await attachmentService.DeleteAttachmentAsync(
                uploadedFile.PublicId);

            return Result<Guid>.Failure(attachmentResult.Error!);
        }

        // 4. Add attachment as Pending
        attachmentRepository.Add(attachmentResult.Value);

        // 5. Create Product using Attachment ID
        var productId = Guid.NewGuid();

        var productResult =
            Product.Create(
                productId,
                request.Name,
                request.Description,
                attachmentId,
                request.Price,
                request.ProductBrandId,
                request.ProductTypeId);

        if (productResult.IsFailure)
        {
            await attachmentService.DeleteAttachmentAsync(
                uploadedFile.PublicId);

            return Result<Guid>.Failure(productResult.Error!);
        }

        // 6. Add Product
        productRepository.Add(productResult.Value);

        // 7. Mark attachment as linked
        var linkResult =
            attachmentResult.Value.MarkAsLinked();

        if (linkResult.IsFailure)
        {
            await attachmentService.DeleteAttachmentAsync(
                uploadedFile.PublicId);

            return Result<Guid>.Failure(linkResult.Error!);
        }

        // 8. Save everything
        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch
        {
            // DB failed, Cloudinary file would otherwise become orphaned
            await attachmentService.DeleteAttachmentAsync(
                uploadedFile.PublicId);

            throw;
        }

        return Result<Guid>.Success(productId);
    }
}