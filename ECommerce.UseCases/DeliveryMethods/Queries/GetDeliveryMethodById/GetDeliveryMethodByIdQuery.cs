using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.UseCases.DeliveryMethods.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethodById;

public sealed record GetDeliveryMethodByIdQuery(Guid Id)
    : IQuery<Result<DeliveryMethodResponse>>;
