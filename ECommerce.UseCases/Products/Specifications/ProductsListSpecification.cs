using ECommerce.Domain.Entities;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

public sealed class ProductsListSpecification:Specification<Product, GetAllProductsResponse>
{
    public ProductsListSpecification()
    {
        Query
            .OrderBy(p => p.Name)
            .Select(p => new GetAllProductsResponse(p.Id, p.Name, p.Description, p.Price, p.PictureAttachment.Url, p.ProductBrand.Name, p.ProductType.Name));
    }
}
