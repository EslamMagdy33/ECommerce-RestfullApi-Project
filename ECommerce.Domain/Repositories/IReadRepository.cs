using ECommerce.Domain.Entities;
using ECommerce.Domain.Specifications;

namespace ECommerce.Domain.Repositories;

public interface IReadRepository<T> where T : BaseEntity
{
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken ct = default);
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default);
    Task<T> SingleAsync(ISpecification<T> specification, CancellationToken ct = default);
    Task<TResult> SingleAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken ct = default);
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken ct = default);
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken ct = default);
    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken ct = default);
    Task<PagedResult<T>> PagedListAsync(ISpecification<T> specification, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListIncludingDeletedAsync(ISpecification<T> specification, CancellationToken ct = default);
}