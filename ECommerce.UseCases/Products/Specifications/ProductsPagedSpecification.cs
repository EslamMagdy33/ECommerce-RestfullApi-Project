using ECommerce.Domain.Entities;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Products.Enums;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

public sealed class ProductsPagedSpecification
    : Specification<Product, GetAllProductsResponse>
{
    public ProductsPagedSpecification(
        string? search = null,
        Guid? brandId = null,
        Guid? typeId = null,
        ProductSortField? sortBy = null,
        bool sortDescending = false,
        int? pageNumber = null,
        int? pageSize = null)
    {
        var query = Query;

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query.Where(p =>
                p.Name.Contains(term) ||
                p.Description.Contains(term));
        }

        // Brand filter
        if (brandId.HasValue)
        {
            query.Where(p =>
                p.ProductBrandId == brandId.Value);
        }

        // Type filter
        if (typeId.HasValue)
        {
            query.Where(p =>
                p.ProductTypeId == typeId.Value);
        }

        // Sorting
        if (sortBy.HasValue)
        {
            ApplySort(
                query,
                sortBy.Value,
                sortDescending);
        }
        else
        {
            query.OrderBy(p => p.Name);
        }

        // Pagination
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var skip =
                (pageNumber.Value - 1) *
                pageSize.Value;

            query
                .Skip(skip)
                .Take(pageSize.Value);
        }

        // Projection
        query.Select(p => new GetAllProductsResponse(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.PictureAttachment.Url,
            p.ProductBrand.Name,
            p.ProductType.Name
        ));
    }

    private static void ApplySort(
        ISpecificationBuilder<Product, GetAllProductsResponse> query,
        ProductSortField sortBy,
        bool sortDescending)
    {
        switch (sortBy)
        {
            case ProductSortField.Name:

                if (sortDescending)
                {
                    query.OrderByDescending(p => p.Name);
                }
                else
                {
                    query.OrderBy(p => p.Name);
                }

                break;

            case ProductSortField.Price:

                if (sortDescending)
                {
                    query
                        .OrderByDescending(p => p.Price)
                        .ThenBy(p => p.Name);
                }
                else
                {
                    query
                        .OrderBy(p => p.Price)
                        .ThenBy(p => p.Name);
                }

                break;

            case ProductSortField.Brand:

                if (sortDescending)
                {
                    query
                        .OrderByDescending(p => p.ProductBrand.Name)
                        .ThenBy(p => p.Name);
                }
                else
                {
                    query
                        .OrderBy(p => p.ProductBrand.Name)
                        .ThenBy(p => p.Name);
                }

                break;

            case ProductSortField.Type:

                if (sortDescending)
                {
                    query
                        .OrderByDescending(p => p.ProductType.Name)
                        .ThenBy(p => p.Name);
                }
                else
                {
                    query
                        .OrderBy(p => p.ProductType.Name)
                        .ThenBy(p => p.Name);
                }

                break;

            default:

                query.OrderBy(p => p.Name);

                break;
        }
    }
}