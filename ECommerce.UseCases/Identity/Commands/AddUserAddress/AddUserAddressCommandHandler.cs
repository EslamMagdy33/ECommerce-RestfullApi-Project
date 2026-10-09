using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;
using System.Net;

namespace ECommerce.UseCases.Identity.Commands.AddUserAddress;

public sealed class AddUserAddressCommandHandler
    (
    ICurrentUserService userService,
    IRepository<UserAddress> userAddressRepository,
    IUnitOfWork unitOfWork )
    : IRequestHandle<AddUserAddressCommand, Result<UserAddressResponse>>
{
    public async Task<Result<UserAddressResponse>> Handle(AddUserAddressCommand request, CancellationToken ct = default)
    {
        if(userService.UserId is null)
            return Result<UserAddressResponse>.Failure(IdentityErrors.InvalidCredentials);

            var createResult = UserAddress.Create(
            Guid.NewGuid(),
            userService.UserId.Value,
            request.Label,
            request.RecipientFirstName,
            request.RecipientLastName,
            request.PhoneNumber,
            request.Country,
            request.City,
            request.Street,
            request.PostalCode,
            request.IsDefaultShipping,
            request.IsDefaultBilling);

            if (createResult.IsFailure)
            {
                return Result<UserAddressResponse>.Failure(createResult.Error);
            }
            var address = createResult.Value;
            userAddressRepository.Add(address);
            await unitOfWork.SaveChangesAsync(ct);


            return Result<UserAddressResponse>.Success(new UserAddressResponse(
            address.Id,
            address.Label,
            address.RecipientFirstName,
            address.RecipientLastName,
            address.PhoneNumber,
            address.Country,
            address.City,
            address.Street,
            address.PostalCode,
            address.IsDefaultShipping,
            address.IsDefaultBilling));
        
    }
}
