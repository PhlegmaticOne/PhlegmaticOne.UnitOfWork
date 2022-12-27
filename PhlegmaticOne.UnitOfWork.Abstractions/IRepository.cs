using System.Linq.Expressions;
using PhlegmaticOne.DomainDefinitions;
using PhlegmaticOne.PagedLists;
using PhlegmaticOne.UnitOfWork.Abstractions.Builders;

namespace PhlegmaticOne.UnitOfWork.Abstractions;

public interface IRepository { }

public interface IRepository<TEntity> : IRepository where TEntity : Entity
{
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<IList<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entity,
        CancellationToken cancellationToken = default);


    Task<bool> DeleteAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(TEntity entity,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);


    Task<TEntity> UpdateAsync(TEntity entity,
        Action<TEntity> actionOverExistingEntity,
        CancellationToken cancellationToken = default);

    Task<TEntity?> UpdateAsync(Guid entityId,
        Action<TEntity> actionOverExistingEntity,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities,
        Action<TEntity> actionOverExistingEntities,
        CancellationToken cancellationToken = default);


    Task<TEntity?> GetByIdOrDefaultAsync(Guid id,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    Task<TResult?> GetByIdOrDefaultAsync<TResult>(Guid id,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    Task<PagedList<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<PagedList<TResult>> GetPagedListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> predicate,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<TProperty> MaxAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<TProperty> MinAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<decimal> AverageAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<decimal> SumAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}