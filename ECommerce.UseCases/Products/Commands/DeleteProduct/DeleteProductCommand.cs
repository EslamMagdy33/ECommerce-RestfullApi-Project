using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id): ICommand<Result>;