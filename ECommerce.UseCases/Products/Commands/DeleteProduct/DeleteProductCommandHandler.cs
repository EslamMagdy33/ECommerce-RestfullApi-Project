using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler
    : IRequestHandle<DeleteProductCommand, Result>
{
    private readonly IRepository<Product> productRepository;
    private readonly IUnitOfWork unitOfWork;

    public DeleteProductCommandHandler(
        IRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        this.productRepository = productRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteProductCommand request,
        CancellationToken ct = default)
    {
        var product =
            await productRepository.GetByIdAsync(
                request.Id,
                ct);

        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        product.MarkAsDeleted();

        productRepository.Update(product);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}