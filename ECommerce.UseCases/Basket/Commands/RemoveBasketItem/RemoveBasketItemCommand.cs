using ECommerce.Domain.Common;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.RemoveBasketItem;

public sealed record RemoveBasketItemCommand(Guid BuyerId, Guid ProductId)
    : ICommand<Result<GetBasketResponse>>;
