using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PhlegmaticOne.DomainDefinitions;
using PhlegmaticOne.PagedLists;
using PhlegmaticOne.PagedLists.Extensions;
using PhlegmaticOne.UnitOfWork.Abstractions;
using PhlegmaticOne.UnitOfWork.Abstractions.Builders;
using PhlegmaticOne.UnitOfWork.Implementation.Builders;

namespace PhlegmaticOne.UnitOfWork.Implementation.Implementation;

public class DbSetRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly DbSet<TEntity> _set;

    public DbSetRepository(DbSet<TEntity> dbSet) => _set = dbSet;

    public async Task<TEntity> CreateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        var result = await _set.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public async Task<IList<TEntity>> CreateRangeAsync(
        IEnumerable<TEntity> entity,
        CancellationToken cancellationToken = default)
    {
        var list = entity as IList<TEntity> ?? entity.ToList();
        await _set.AddRangeAsync(list, cancellationToken);
        return list;
    }


    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdOrDefaultAsync(id, cancellationToken: cancellationToken);

        if (entity is null)
        {
            return false;
        }

        _set.Remove(entity);
        return true;
    }

    public Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        DeleteAsync(entity.Id, cancellationToken);

    public async Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var result = true;
        foreach (var entity in entities)
        {
            result = await DeleteAsync(entity.Id, cancellationToken);
        }
        return result;
    }

    public Task<TEntity> UpdateAsync(TEntity entity,
        Action<TEntity> actionOverExistingEntity,
        CancellationToken cancellationToken = default)
    {
        actionOverExistingEntity(entity);
        _set.Update(entity);
        return Task.FromResult(entity);
    }

    public async Task<TEntity?> UpdateAsync(Guid id,
        Action<TEntity> actionOverExistingEntity,
        CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdOrDefaultAsync(id, cancellationToken: cancellationToken);

        if (existing is null)
        {
            return null;
        }

        actionOverExistingEntity(existing);
        _set.Update(existing);
        return existing;
    }

    public Task<IEnumerable<TEntity>> UpdateRangeAsync(
        IEnumerable<TEntity> entities,
        Action<TEntity> actionOverExistingEntities,
        CancellationToken cancellationToken = default)
    {
        var entityBases = entities as IList<TEntity> ?? entities.ToList();
        foreach (var entity in entityBases)
        {
            actionOverExistingEntities(entity);
        }
        _set.UpdateRange(entityBases);
        return Task.FromResult((IEnumerable<TEntity>)entityBases);
    }


    public Task<TEntity?> GetByIdOrDefaultAsync(Guid id,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null, 
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set;

        if (disableTracking) query = query.AsNoTracking();

        if (include is not null)  query = Include(query, include); 

        if (predicate is not null) query = query.Where(predicate);

        return query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TResult?> GetByIdOrDefaultAsync<TResult>(Guid id,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set;

        if (disableTracking) query = query.AsNoTracking();

        if (include is not null) query = Include(query, include);

        if (predicate is not null) query = query.Where(predicate);

        return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<IList<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set;

        if (disableTracking) query = query.AsNoTracking();

        if (include is not null) query = Include(query, include);

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        return await query.Select(selector).ToListAsync(cancellationToken);
    }

    public async Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set;

        if (disableTracking) query = query.AsNoTracking();

        if (include is not null) query = Include(query, include);

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        return await query.ToListAsync(cancellationToken);
    }

    public Task<PagedList<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set;

        if (disableTracking) query = query.AsNoTracking();

        if (include is not null) query = Include(query, include);

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        return query.ToPagedListAsync(pageIndex, pageSize, cancellationToken: cancellationToken);
    }

    public async Task<PagedList<TResult>> GetPagedListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set;

        if (disableTracking) query = query.AsNoTracking();

        if (include is not null) query = Include(query, include);

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        return await query.Select(selector).ToPagedListAsync(pageIndex, pageSize, cancellationToken: cancellationToken);
    }

    public Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set;

        if (disableTracking) query = query.AsNoTracking();

        if (include is not null) query = Include(query, include);

        return query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> predicate,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set;

        if (disableTracking) query = query.AsNoTracking();

        if (include is not null) query = Include(query, include);

        return await query.Where(predicate).Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null ? _set.CountAsync(cancellationToken) : _set.CountAsync(predicate, cancellationToken);

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null ? _set.AnyAsync(cancellationToken) : _set.AnyAsync(predicate, cancellationToken);

    public Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        _set.AllAsync(predicate, cancellationToken);

    public Task<TProperty> MaxAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _set.MaxAsync(selector, cancellationToken: cancellationToken)
            : _set.Where(predicate).MaxAsync(selector, cancellationToken: cancellationToken);

    public Task<TProperty> MinAsync<TProperty>(Expression<Func<TEntity, TProperty>> selector, Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _set.MinAsync(selector, cancellationToken: cancellationToken)
            : _set.Where(predicate).MinAsync(selector, cancellationToken: cancellationToken);

    public Task<decimal> AverageAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _set.AverageAsync(selector, cancellationToken)
            : _set.Where(predicate).AverageAsync(selector, cancellationToken);

    public Task<decimal> SumAsync(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null
            ? _set.SumAsync(selector, cancellationToken)
            : _set.Where(predicate).SumAsync(selector, cancellationToken);

    private IQueryable<TEntity> Include(IQueryable<TEntity> queryable,
        Func<IIncludeQueryBuilder<TEntity>, IQueryable<TEntity>>? include = null)
    {
        if (include is null)
        {
            return queryable;
        }

        var includeQueryBuilder = new IncludeQueryBuilder<TEntity>(queryable);
        return include(includeQueryBuilder);
    }
}

