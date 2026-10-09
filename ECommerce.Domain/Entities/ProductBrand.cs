using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;

namespace ECommerce.Domain.Entities;

public class ProductBrand : BaseEntity
{
    public string Name { get; private set; } = null!;

    // Navigational Properties
    public ICollection<Product> Products { get; private set; } = [];

    private ProductBrand() { }
    public static Result<ProductBrand> Create(Guid id,string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        if (id == Guid.Empty)
            return Result<ProductBrand>.Failure(ProductBrandErrors.InvalidId);
        var productBrand = new ProductBrand
        {
            Id = id,
            Name = name.Trim()
        };
        return Result<ProductBrand>.Success(productBrand);
    }


}
