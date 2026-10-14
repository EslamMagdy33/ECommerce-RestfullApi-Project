using ECommerce.Domain.Entities;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

public sealed class ProductForBasketSpecification : Specification<Product,ProductForBasketResponse>
{
    public ProductForBasketSpecification(IEnumerable<Guid> productIds)
    {
        var ids=productIds.Distinct().ToList();
        Query
            .Where(product => ids.Contains(product.Id))
            .Select(product => new ProductForBasketResponse(
                               product.Id,
                               product.Name,
                               product.PictureAttachment.Url,
                               product.Price));
    }
}
