using ECommerce.Domain.Entities;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

public sealed class ProductForBasketSpecification : Specification<Product,ProductForBasketResponse>
{
    public ProductForBasketSpecification(Guid productId)
    {
        Query
            .Where(product => product.Id == productId)
            .Select(product => new ProductForBasketResponse(
                               product.Id,
                               product.Name,
                               product.PictureAttachment.Url,
                               product.Price));
    }
}
