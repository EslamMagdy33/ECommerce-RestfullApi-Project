using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.DeliveryMethods.DTOs;
using ECommerce.UseCases.DeliveryMethods.Specifications;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Commands.CreateDeliveryMethod;

public sealed class CreateDeliveryMethodCommandHandler(
    IRepository<DeliveryMethod> repository,IUnitOfWork unitOfWork)
    : IRequestHandle<CreateDeliveryMethodCommand, Result<DeliveryMethodResponse>>
{
    public async Task<Result<DeliveryMethodResponse>> Handle(CreateDeliveryMethodCommand request, CancellationToken ct = default)
    {
        var itemName = await repository.AnyAsync(new DeliveryMethodByNameSpecification(request.Name), ct);
        if (itemName)
            return Result<DeliveryMethodResponse>.Failure(DeliveryMethodErrors.NameAlreadyExists);

        var createDelivery = DeliveryMethod.Create(
            Guid.NewGuid(), request.Name, request.Price, request.EstimatedDeliveryTime, request.Description,
            request.IsAvailable, request.DisplayOrder);

        if (createDelivery.IsFailure)
            return Result<DeliveryMethodResponse>.Failure(createDelivery.Error);

        repository.Add(createDelivery.Value);

        await unitOfWork.SaveChangesAsync(ct);
        return Result<DeliveryMethodResponse>.Success(ToResponse(createDelivery.Value));
    }
    private static DeliveryMethodResponse ToResponse(DeliveryMethod m) =>
        new(m.Id, m.Name, m.Description, m.Price, m.EstimatedDeliveryTime, m.IsAvailable, m.DisplayOrder);
}
