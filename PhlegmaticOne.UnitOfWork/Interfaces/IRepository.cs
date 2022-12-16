using Microsoft.EntityFrameworkCore.Query;
using PhlegmaticOne.PagedLists.Implementation;
using PhlegmaticOne.UnitOfWork.Models;
using System.Linq.Expressions;

namespace PhlegmaticOne.UnitOfWork.Interfaces;

public interface IRepository { }

public interface IRepository<TEntity> : IRepository where TEntity : EntityBase
{
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = new());
    Task<IList<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = new());

    Task<TEntity> UpdateAsync(TEntity entity, Action<TEntity> actionOverExistingEntity,
        CancellationToken cancellationToken = new());

    Task<TEntity?> UpdateAsync(Guid entityId, Action<TEntity> actionOverExistingEntity,
        CancellationToken cancellationToken = new());

    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities,
        Action<TEntity> actionOverExistingEntities, CancellationToken cancellationToken = new());

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = new());

    Task<TEntity?> GetByIdOrDefaultAsync(Guid id,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new());

    Task<TResult?> GetByIdOrDefaultAsync<TResult>(Guid id,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new());

    Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new());

    Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new());

    Task<PagedList<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = new());

    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new());

    Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new());

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = new());

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = new());

    Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = new());
}