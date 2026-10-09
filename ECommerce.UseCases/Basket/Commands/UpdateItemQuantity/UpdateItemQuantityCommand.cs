using ECommerce.Domain.Common;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.UpdateItemQuantity;

public sealed record UpdateItemQuantityCommand(Guid BuyerId, Guid ProductId, int Quantity)
    : ICommand<Result<GetBasketResponse>>;
