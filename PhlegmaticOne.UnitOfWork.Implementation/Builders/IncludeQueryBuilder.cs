using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PhlegmaticOne.UnitOfWork.Abstractions.Builders;

namespace PhlegmaticOne.UnitOfWork.Implementation.Builders;

internal class IncludeQueryBuilder<TEntity> : IIncludeQueryBuilder<TEntity> where TEntity : class
{
    private readonly IQueryable<TEntity> _queryable;
    internal IncludeQueryBuilder(IQueryable<TEntity> queryable) => _queryable = queryable;

    public IThenIncludeQueryBuilder<TEntity, TProperty> Include<TProperty>(
        Expression<Func<TEntity, TProperty>> includeExpression) where TProperty : class
    {
        var includable = _queryable.Include(includeExpression);
        return new ThenIncludeQueryBuilder<TEntity, TProperty>(includable);
    }

    public IQueryable<TEntity> Build() => _queryable;
}