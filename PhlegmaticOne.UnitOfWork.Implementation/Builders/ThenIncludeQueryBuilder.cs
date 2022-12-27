using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PhlegmaticOne.UnitOfWork.Abstractions.Builders;

namespace PhlegmaticOne.UnitOfWork.Implementation.Builders;

internal class ThenIncludeQueryBuilder<TEntity, TProperty> : IThenIncludeQueryBuilder<TEntity, TProperty>
    where TEntity : class
    where TProperty : class
{
    private readonly IIncludableQueryable<TEntity, TProperty> _queryable;

    internal ThenIncludeQueryBuilder(IIncludableQueryable<TEntity, TProperty> queryable) => _queryable = queryable;

    public IThenIncludeQueryBuilder<TEntity, TNext> ThenInclude<TNext>(
        Expression<Func<TProperty, TNext>> thenIncludeExpression) where TNext : class
    {
        var includable = _queryable.ThenInclude(thenIncludeExpression);
        return new ThenIncludeQueryBuilder<TEntity, TNext>(includable);
    }

    public IThenIncludeQueryBuilder<TEntity, TNew> Include<TNew>(
        Expression<Func<TEntity, TNew>> includeExpression) where TNew : class
    {
        var includable = _queryable.Include(includeExpression);
        return new ThenIncludeQueryBuilder<TEntity, TNew>(includable);
    }

    public IQueryable<TEntity> Build() => _queryable;
}