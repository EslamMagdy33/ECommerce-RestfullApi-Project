using ECommerce.Domain.Entities;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

public sealed class AllProductsForAttachmentCleanupSpecification : Specification<Product>
{
    public AllProductsForAttachmentCleanupSpecification()
    {
        // Intentionally empty.
        // IgnoreQueryFilters() allows both active
        // and soft-deleted products to be returned.
    }
}