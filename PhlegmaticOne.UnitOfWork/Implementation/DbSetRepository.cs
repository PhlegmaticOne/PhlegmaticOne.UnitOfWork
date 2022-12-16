using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PhlegmaticOne.PagedLists.Extensions;
using PhlegmaticOne.PagedLists.Implementation;
using PhlegmaticOne.UnitOfWork.Interfaces;
using PhlegmaticOne.UnitOfWork.Models;
using System.Linq.Expressions;

namespace PhlegmaticOne.UnitOfWork.Implementation;

public class DbSetRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
{
    private readonly DbSet<TEntity> _set;

    public DbSetRepository(DbSet<TEntity> dbSet)
    {
        _set = dbSet;
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = new())
    {
        var result = await _set.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public async Task<IList<TEntity>> CreateRangeAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = new CancellationToken())
    {
        var list = entity as IList<TEntity> ?? entity.ToList();
        await _set.AddRangeAsync(list, cancellationToken);
        return list;
    }

    public Task<TEntity> UpdateAsync(TEntity entity,
        Action<TEntity> actionOverExistingEntity,
        CancellationToken cancellationToken = new())
    {
        actionOverExistingEntity(entity);
        _set.Update(entity);
        return Task.FromResult(entity);
    }

    public async Task<TEntity?> UpdateAsync(Guid id,
        Action<TEntity> actionOverExistingEntity,
        CancellationToken cancellationToken = new())
    {
        var existing = await GetByIdOrDefaultAsync(id, cancellationToken: cancellationToken);

        if (existing is null) return null;

        actionOverExistingEntity(existing);
        _set.Update(existing);
        return existing;
    }

    public Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities,
        Action<TEntity> actionOverExistingEntities,
        CancellationToken cancellationToken = new())
    {
        var entityBases = entities as TEntity[] ?? entities.ToArray();
        foreach (var entity in entityBases) actionOverExistingEntities(entity);
        _set.UpdateRange(entityBases);
        return Task.FromResult((IEnumerable<TEntity>)entityBases);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var entity = await GetByIdOrDefaultAsync(id, cancellationToken: cancellationToken);

        if (entity is null) return false;

        _set.Remove(entity);
        return true;
    }

    public Task<TEntity?> GetByIdOrDefaultAsync(Guid id,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        IQueryable<TEntity> query = _set;

        if (include is not null) query = include(query);

        if (predicate is not null) query = query.Where(predicate);

        return query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TResult?> GetByIdOrDefaultAsync<TResult>(Guid id,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        IQueryable<TEntity> query = _set;

        if (include is not null) query = include(query);

        if (predicate is not null) query = query.Where(predicate);

        return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        IQueryable<TEntity> query = _set;

        if (include is not null) query = include(query);

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        return await query.Select(selector).ToListAsync(cancellationToken);
    }

    public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        IQueryable<TEntity> query = _set;

        if (include is not null) query = include(query);

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        return await query.ToListAsync(cancellationToken);
    }

    public Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = new())
    {
        IQueryable<TEntity> query = _set;

        if (include is not null) query = include(query);

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        return query.ToPagedListAsync(pageIndex, pageSize, cancellationToken: cancellationToken);
    }

    public Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        IQueryable<TEntity> query = _set;

        if (include is not null) query = include(query);

        return query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        IQueryable<TEntity> query = _set;

        if (include is not null) query = include(query);

        return await query.Where(predicate).Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = new())
    {
        return predicate is null ? _set.CountAsync(cancellationToken) : _set.CountAsync(predicate, cancellationToken);
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = new())
    {
        return predicate is null ? _set.AnyAsync(cancellationToken) : _set.AnyAsync(predicate, cancellationToken);
    }

    public Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = new())
    {
        return _set.AllAsync(predicate, cancellationToken);
    }
}