using ECommerce.UseCases.Brands.DTOs;

namespace ECommerce.UseCases.Brands;

public interface IProductBrandQueryService
{
    Task<IReadOnlyList<GetAllBrandsResponse>> GetAllBrandsAsync(CancellationToken ct=default);
}
