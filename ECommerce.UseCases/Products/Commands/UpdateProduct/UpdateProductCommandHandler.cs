using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler
    : IRequestHandle<UpdateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IAttachmentService attachmentService;
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<UploadedAttachment> attachmentRepository;

    public UpdateProductCommandHandler(
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
        UpdateProductCommand request,
        CancellationToken ct = default)
    {
        // 1. Get product
        var product = await productRepository.GetByIdAsync(request.Id, ct);

        if (product is null)
            return Result<Guid>.Failure(ProductErrors.NotFound);

        // 2. Check duplicate name
        var existingProduct =
            await productRepository.AnyAsync(
                new ProductByNameSpecification(
                    request.Name,
                    request.Id),
                ct);

        if (existingProduct)
            return Result<Guid>.Failure(ProductErrors.DuplicateName);

        // 3. Keep old attachment ID
        var oldAttachmentId = product.PictureAttachmentId;

        // 4. Update normal product data
        var renameResult = product.Rename(request.Name);

        if (renameResult.IsFailure)
            return Result<Guid>.Failure(renameResult.Error!);

        var descriptionResult =
            product.ChangeDescription(request.Description);

        if (descriptionResult.IsFailure)
            return Result<Guid>.Failure(descriptionResult.Error!);

        var priceResult =
            product.ChangePrice(request.Price);

        if (priceResult.IsFailure)
            return Result<Guid>.Failure(priceResult.Error!);

        var brandResult =
            product.ChangeBrand(request.ProductBrandId);

        if (brandResult.IsFailure)
            return Result<Guid>.Failure(brandResult.Error!);

        var typeResult =
            product.ChangeType(request.ProductTypeId);

        if (typeResult.IsFailure)
            return Result<Guid>.Failure(typeResult.Error!);

        // 5. If there is NO new image
        if (request.Image is null)
        {
            productRepository.Update(product);

            await unitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(product.Id);
        }

        // 6. Upload new image
        var uploadResult =
            await attachmentService.UploadAttachmentAsync(request.Image);

        if (uploadResult.IsFailure)
            return Result<Guid>.Failure(uploadResult.Error!);

        var uploadedFile = uploadResult.Value;

        // 7. Create new attachment as Pending
        var newAttachmentId = Guid.NewGuid();

        var attachmentResult =
            UploadedAttachment.Create(
                newAttachmentId,
                uploadedFile.PublicId,
                uploadedFile.Url);

        if (attachmentResult.IsFailure)
        {
            await attachmentService.DeleteAttachmentAsync(
                uploadedFile.PublicId);

            return Result<Guid>.Failure(
                attachmentResult.Error!);
        }

        attachmentRepository.Add(
            attachmentResult.Value);

        // 8. Change Product to new attachment
        var pictureResult =
            product.ChangePictureAttachment(newAttachmentId);

        if (pictureResult.IsFailure)
        {
            await attachmentService.DeleteAttachmentAsync(
                uploadedFile.PublicId);

            return Result<Guid>.Failure(
                pictureResult.Error!);
        }

        // 9. Mark new attachment as Linked
        var linkResult =
            attachmentResult.Value.MarkAsLinked();

        if (linkResult.IsFailure)
        {
            await attachmentService.DeleteAttachmentAsync(
                uploadedFile.PublicId);

            return Result<Guid>.Failure(
                linkResult.Error!);
        }

        productRepository.Update(product);

        // 10. Save DB
        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {

            Console.WriteLine("========== UPDATE ERROR ==========");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);
            Console.WriteLine(ex.InnerException?.InnerException?.Message);
            Console.WriteLine("=================================");

            await attachmentService.DeleteAttachmentAsync(
                uploadedFile.PublicId);

            throw;
        }

        // 11. Old attachment cleanup
        // Don't delete it before DB succeeds.
        // We'll handle orphan cleanup separately.

        return Result<Guid>.Success(product.Id);
    }
}