using ECommerce.UseCases.Brands.Queries;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Queries.GetAllProducts;
using ECommerce.UseCases.Products.Queries.GetByIdProduct;
using ECommerce.UseCases.Products.Queries.GetPagedProducts;
using ECommerce.UseCases.ProductTypes.Queries;
using ECommerce.UseCases.Utility;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.UseCases
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection service)
        {
            service.AddMessaging(Assembly.GetExecutingAssembly());
            service.AddValidatorsFromAssembly(typeof(GetPagedProductsQueryValidator).Assembly);


            return service;
        }
    }
}
