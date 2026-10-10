using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.Models;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Products.Commands.CreateProduct;

public sealed record CreateProductCommand
    (
    string Name,
    string Description,
    decimal Price,
    FileUpload Image,
    Guid ProductBrandId,
    Guid ProductTypeId
    ) : ICommand<Result<Guid>>;

