using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.DeliveryMethods.DTOs;
using ECommerce.UseCases.DeliveryMethods.Specifications;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Commands.UpdateDeliveryMethod;

public sealed class UpdateDeliveryMethodCommandHandler(
    IRepository<DeliveryMethod> repository,IUnitOfWork unitOfWork)
    : IRequestHandle<UpdateDeliveryMethodCommand, Result<DeliveryMethodResponse>>
{
    public async Task<Result<DeliveryMethodResponse>> Handle(UpdateDeliveryMethodCommand request, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Result<DeliveryMethodResponse>.Failure(DeliveryMethodErrors.NotFound);

        var Exist = await repository.AnyAsync(new DeliveryMethodByNameSpecification(request.Name, request.Id), ct);
        if (Exist)
            return Result<DeliveryMethodResponse>.Failure(DeliveryMethodErrors.NameAlreadyExists);

        var updateResult = entity.Update(
            request.Name, request.Price, request.EstimatedDeliveryTime, request.Description,
            request.IsAvailable, request.DisplayOrder);

        if (updateResult.IsFailure)
            return Result<DeliveryMethodResponse>.Failure(updateResult.Error);

        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<DeliveryMethodResponse>.Success(
            new DeliveryMethodResponse(
                entity.Id, entity.Name, entity.Description, entity.Price, entity.EstimatedDeliveryTime,
                entity.IsAvailable, entity.DisplayOrder));
    }

}
