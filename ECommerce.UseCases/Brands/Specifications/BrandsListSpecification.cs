using ECommerce.Domain.Entities;
using ECommerce.UseCases.Brands.DTOs;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Brands.Specifications;

public sealed class BrandsListSpecification: Specification<ProductBrand,GetAllBrandsResponse>
{
    public BrandsListSpecification()
    {
        Query
            .OrderBy(b=>b.Name)
            .Select(b=>new GetAllBrandsResponse(b.Id, b.Name));
    }
}
