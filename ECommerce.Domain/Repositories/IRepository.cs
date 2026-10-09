using ECommerce.Domain.Entities;
using ECommerce.Domain.Specifications;

namespace ECommerce.Domain.Repositories;

public interface IRepository<T> :IReadRepository<T> where T :BaseEntity
{
    Task<T?> GetByIdAsync(Guid Id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    void Add(T  entity);
    void Update(T entity);
    void Delete(T entity);
}
