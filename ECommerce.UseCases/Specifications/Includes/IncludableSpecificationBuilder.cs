using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications.Includes;

public sealed class IncludableSpecificationBuilder<T, TProperty> :
    SpecificationBuilder<T>, IIncludableSpecificationBuilder<T, TProperty>
{
    private readonly LambdaExpression _parent;

    public IncludableSpecificationBuilder
        (Specification<T> specification, LambdaExpression parent) : base(specification)
    {
        _parent = parent;
    }

    public IIncludableSpecificationBuilder<T, TNext> ThenInclude<TNext>(Expression<Func<TProperty, TNext>> navigation)
    {
        Specification.AddThenInclude(navigation, _parent);
        return new IncludableSpecificationBuilder<T, TNext>(Specification, navigation);
    }
}
public sealed class IncludableCollectionSpecificationBuilder<T, TElement> :
    SpecificationBuilder<T>, IIncludeCollectionSpecificationBuilder<T, TElement> 
{
    private readonly LambdaExpression _parent;

    public IncludableCollectionSpecificationBuilder
        (Specification<T> specification, LambdaExpression parent) : base(specification)
    {
        _parent = parent;
    }

    public IIncludableSpecificationBuilder<T, TNext> ThenInclude<TNext>(Expression<Func<TElement, TNext>> navigation)
    {
        Specification.AddThenInclude(navigation, _parent);
        return new IncludableSpecificationBuilder<T, TNext>(Specification, navigation);
    }
}

