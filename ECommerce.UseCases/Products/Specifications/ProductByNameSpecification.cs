using ECommerce.Domain.Entities;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

public sealed class ProductByNameSpecification:Specification<Product>
{
    public ProductByNameSpecification(string name, Guid? excludeProductId = null)
    {
        var normalizedName = name.Trim();
        Query
            .Where(p => p.Name == normalizedName && (!excludeProductId.HasValue || p.Id != excludeProductId.Value));
    }
        
}
