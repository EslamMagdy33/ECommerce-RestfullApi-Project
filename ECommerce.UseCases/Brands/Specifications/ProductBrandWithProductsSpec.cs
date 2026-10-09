using ECommerce.Domain.Entities;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Brands.Specifications;

public sealed class ProductBrandWithProductsSpec:Specification<ProductBrand>
{
    public ProductBrandWithProductsSpec(Guid brndId)
    {
        Query
            .Where(b=>b.Id== brndId)
            .Include(b => b.Products)
                .ThenInclude(p=>p.ProductType)
            .AsNoTracking();
    }
}
