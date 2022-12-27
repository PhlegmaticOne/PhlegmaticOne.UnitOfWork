using System.Linq.Expressions;

namespace PhlegmaticOne.UnitOfWork.Abstractions.Builders;

public interface IThenIncludeQueryBuilder<TEntity, TProperty> : IIncludeQueryBuilder<TEntity>
    where TEntity : class
    where TProperty : class
{
    IThenIncludeQueryBuilder<TEntity, TNext> ThenInclude<TNext>(
        Expression<Func<TProperty, TNext>> thenIncludeExpression) where TNext : class;
}