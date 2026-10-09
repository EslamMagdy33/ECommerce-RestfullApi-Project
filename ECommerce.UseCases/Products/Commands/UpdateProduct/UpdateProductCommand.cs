using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ECommerce.UseCases.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IFormFile? Image,
    Guid ProductBrandId,
    Guid ProductTypeId)
    : ICommand<Result<Guid>>;