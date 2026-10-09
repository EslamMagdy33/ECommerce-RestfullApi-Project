using ECommerce.Domain.Common;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.AddBasketItem;

public sealed record AddItemToBasketCommand(Guid BuyerId,Guid ProductId,int Quantity) : ICommand<Result<GetBasketResponse>>;

