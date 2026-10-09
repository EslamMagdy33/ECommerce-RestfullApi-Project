using ECommerce.Domain.Entities;
using ECommerce.UseCases.ProductTypes.DTOs;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.ProductTypes.Specifications;

public sealed class TypesListSpecification: Specification<ProductType, GetAllProductTypesResponse>
{
    public TypesListSpecification()
    {
        Query
            .OrderBy(type => type.Name)
            .Select(type => new GetAllProductTypesResponse(type.Id, type.Name));
    }
}
