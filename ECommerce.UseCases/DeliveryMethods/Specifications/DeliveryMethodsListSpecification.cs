using ECommerce.Domain.Entities;
using ECommerce.UseCases.DeliveryMethods.DTOs;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.DeliveryMethods.Specifications;

public sealed class DeliveryMethodsListSpecification : Specification<DeliveryMethod,DeliveryMethodResponse>
{
    public DeliveryMethodsListSpecification(bool availableOnly , string? search )
    {
        if (availableOnly)
            Query.Where(m => m.IsAvailable);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            Query.Where(m => m.Name.ToLower().Contains(searchLower)
                          || (m.Description != null && m.Description.ToLower().Contains(searchLower)));
        }
        Query
            .OrderBy(m=>m.DisplayOrder)
            .Select(m=>new DeliveryMethodResponse(
                m.Id,
                m.Name,
                m.Description,
                m.Price,
                m.EstimatedDeliveryTime,
                m.IsAvailable,
                m.DisplayOrder));
    }
}

public sealed class DeliveryMethodByIdSpecification : Specification<DeliveryMethod, DeliveryMethodResponse>
{
    public DeliveryMethodByIdSpecification(Guid id)
    {
        Query
            .Where(m => m.Id == id)
            .Select(m => new DeliveryMethodResponse(
                m.Id,
                m.Name,
                m.Description,
                m.Price,
                m.EstimatedDeliveryTime,
                m.IsAvailable,
                m.DisplayOrder));
    }
}
public sealed class DeliveryMethodByNameSpecification : Specification<DeliveryMethod>
{
    public DeliveryMethodByNameSpecification(string name, Guid? excludeId = null)
    {
        var normalized = name.Trim();
        Query.Where(m => m.Name == normalized);

        if (excludeId.HasValue)
            Query.Where(m => m.Id != excludeId.Value);
    }
}
