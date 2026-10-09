using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Commands.DeleteDeliveryMethod;

public sealed class DeleteDeliveryMethodCommandHnadler(
    IRepository<DeliveryMethod> repository,IUnitOfWork unitOfWork )
    : IRequestHandle<DeleteDeliveryMethodCommand, Result>
{
    public async Task<Result> Handle(DeleteDeliveryMethodCommand request, CancellationToken ct = default)
    {
        var entity=await repository.GetByIdAsync(request.Id,ct);
        if (entity == null)
            return Result.Failure(DeliveryMethodErrors.NotFound);

        repository.Delete(entity);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
