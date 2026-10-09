using ECommerce.Domain.Common;
using ECommerce.UseCases.DeliveryMethods.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Commands.UpdateDeliveryMethod;

public sealed record UpdateDeliveryMethodCommand(
    Guid Id, string Name, decimal Price, string EstimatedDeliveryTime, string? Description = null,
    bool IsAvailable = true, int DisplayOrder = 0)
    : ICommand<Result<DeliveryMethodResponse>>;
