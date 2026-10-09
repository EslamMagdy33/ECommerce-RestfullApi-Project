using ECommerce.Domain.Common;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.ClearBasket;

public sealed record ClearBasketCommand(Guid BuyerId) : ICommand<Result<GetBasketResponse>>;
