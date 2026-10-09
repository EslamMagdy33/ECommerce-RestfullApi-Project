using ECommerce.Domain.Entities;
using System.Linq.Expressions;

namespace ECommerce.Domain.Specifications;

public interface ISpecification<T> 
{
    IReadOnlyList<Expression<Func<T,bool>>> whereExpressions {  get; }
    IReadOnlyList<Expression<Func<T,object>>> Includes { get; }
    IReadOnlyList<OrderExpressionInfo<T>> OrderExpressions { get; }
    IReadOnlyList<IncludeExpressionInfo> IncludeExpressions { get; } // ThenInclude

    #region Pagination
    int? Skip { get; }
    int? Take { get; } 
    bool IsPagingEnabled { get; }
    bool IsTrackingEnabled { get; } 

    #endregion
}
public interface ISpecification<T,TResult> :ISpecification<T> 
{
    Expression<Func<T, TResult>> Selector { get; }
    Expression<Func<T,IEnumerable<TResult>>> SelectorMany { get; }

}
