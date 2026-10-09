using ECommerce.Domain.Common;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.MergeBasket;

public sealed record MergeBasketCommand(Guid BuyerId, Guid AnonymousBuyerId)
    : ICommand<Result<GetBasketResponse>>;