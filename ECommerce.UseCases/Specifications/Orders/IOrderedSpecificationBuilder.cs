using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications.Orders;

public interface IOrderedSpecificationBuilder<T> : ISpecificationBuilder<T> 
{
    IOrderedSpecificationBuilder<T> ThenBy(Expression<Func<T, object?>> keySelector);
    IOrderedSpecificationBuilder<T> ThenByDescending(Expression<Func<T, object?>> keySelector);
}
public interface IOrderedSpecificationBuilder<T, TResult> : ISpecificationBuilder<T, TResult>
{
    IOrderedSpecificationBuilder<T, TResult> ThenBy(Expression<Func<T, object?>> keySelector);
    IOrderedSpecificationBuilder<T, TResult> ThenByDescending(Expression<Func<T, object?>> keySelector);
}
