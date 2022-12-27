using System.Linq.Expressions;

namespace PhlegmaticOne.UnitOfWork.Abstractions.Builders;

public interface IIncludeQueryBuilder<TEntity> : IQueryBuilder<TEntity> where TEntity : class
{
    IThenIncludeQueryBuilder<TEntity, TProperty> Include<TProperty>(
        Expression<Func<TEntity, TProperty>> includeExpression) where TProperty : class;
}