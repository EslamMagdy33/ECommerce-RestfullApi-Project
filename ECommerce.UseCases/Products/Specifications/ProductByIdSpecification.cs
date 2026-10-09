using ECommerce.Domain.Entities;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

public sealed class ProductByIdSpecification:Specification<Product,GetByIdProductResponse>
{
    public ProductByIdSpecification(Guid Id)
    {
        Query
            .Where(p => p.Id == Id)
            .Select(p => new GetByIdProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.PictureAttachment.Url,
                p.ProductBrand.Name,
                p.ProductType.Name)
            );
    }
}
