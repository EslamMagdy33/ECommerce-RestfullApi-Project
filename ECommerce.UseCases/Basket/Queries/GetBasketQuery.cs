using ECommerce.Domain.Common;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Queries;

public sealed record GetBasketQuery(Guid BuyerId) : IQuery<Result<GetBasketResponse>>;
