using ECommerce.Domain.Entities;
using ECommerce.UseCases.Brands.DTOs;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.ProductTypes.DTOs;
using Mapster;

namespace ECommerce.UseCases.Utility;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, GetByIdProductResponse>()
            .Map(dest => dest.ProductBrand, src => src.ProductBrand.Name)
            .Map(dest => dest.ProductType, src => src.ProductType.Name);

        config.NewConfig<Product,GetAllProductsResponse>()
            .Map(dest=>dest.ProductType, src => src.ProductType.Name)
            .Map(dest=>dest.ProductBrand,src=>src.ProductBrand.Name);

        config.NewConfig<ProductBrand, GetAllBrandsResponse>();
        config.NewConfig<ProductType, GetAllProductTypesResponse>();
    }
}
