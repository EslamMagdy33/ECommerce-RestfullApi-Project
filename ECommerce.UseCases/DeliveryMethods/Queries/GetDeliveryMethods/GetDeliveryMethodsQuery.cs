using ECommerce.Domain.Common;
using ECommerce.UseCases.DeliveryMethods.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethods;

public sealed record GetDeliveryMethodsQuery( bool AvailableOnly = true, string? Search = null)
    : IQuery<Result<IReadOnlyList<DeliveryMethodResponse>>>;