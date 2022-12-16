using Microsoft.EntityFrameworkCore.Query;
using PhlegmaticOne.PagedLists.Extensions;
using PhlegmaticOne.PagedLists.Implementation;
using PhlegmaticOne.UnitOfWork.Interfaces;
using PhlegmaticOne.UnitOfWork.Models;
using System.Linq.Expressions;

namespace PhlegmaticOne.UnitOfWork.Implementation;

public class InMemoryRepository<T> : IRepository<T> where T : EntityBase
{
    private readonly List<T> _entities;
    public InMemoryRepository() => _entities = new();
    public InMemoryRepository(IEnumerable<T> entities) => _entities = entities.ToList();

    public Task<T> CreateAsync(T entity, CancellationToken cancellationToken = new())
    {
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<IList<T>> CreateRangeAsync(IEnumerable<T> entity, CancellationToken cancellationToken = new())
    {
        var list = entity as IList<T> ?? entity.ToList();
        _entities.AddRange(list);
        return Task.FromResult(list);
    }

    public Task<T> UpdateAsync(T entity, Action<T> actionOverExistingEntity,
        CancellationToken cancellationToken = new())
    {
        if (_entities.Remove(entity))
        {
            actionOverExistingEntity(entity);
            _entities.Add(entity);
        }
        return Task.FromResult(entity);
    }

    public Task<T?> UpdateAsync(Guid entityId, Action<T> actionOverExistingEntity,
        CancellationToken cancellationToken = new())
    {
        var existing = GetById(entityId);

        if (existing is null) return null;

        return UpdateAsync(existing, actionOverExistingEntity, cancellationToken)!;
    }

    public Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, Action<T> actionOverExistingEntities,
        CancellationToken cancellationToken = new())
    {
        var entityBases = entities as T[] ?? entities.ToArray();

        foreach (var entity in entityBases)
        {
            UpdateAsync(entity, actionOverExistingEntities, cancellationToken);
        }

        return Task.FromResult((IEnumerable<T>)entityBases); ;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var entity = GetById(id);
        return Task.FromResult(_entities.Remove(entity));
    }

    public Task<T?> GetByIdOrDefaultAsync(Guid id,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        var query = _entities.AsQueryable();

        if (predicate is not null) query = query.Where(predicate);

        var result = query.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(result);
    }

    public Task<TResult?> GetByIdOrDefaultAsync<TResult>(Guid id,
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        var query = _entities.AsQueryable();

        if (predicate is not null) query = query.Where(predicate);

        var result = query.Select(selector).FirstOrDefault();
        return Task.FromResult(result);
    }

    public Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        var query = _entities.AsQueryable();

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        IList<TResult> result = query.Select(selector).ToList();

        return Task.FromResult(result);
    }

    public Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        var query = _entities.AsQueryable();

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        IList<T> result = query.ToList();

        return Task.FromResult(result);
    }

    public Task<PagedList<T>> GetPagedListAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = new())
    {
        var query = _entities.AsQueryable();

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) query = orderBy(query);

        var pagedList = query.ToPagedList(pageIndex, pageSize);

        return Task.FromResult(pagedList);
    }

    public Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        var query = _entities.AsQueryable();

        if (include is not null) query = include(query);

        var result = query.FirstOrDefault(predicate);

        return Task.FromResult(result);
    }

    public Task<TResult?> GetFirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        CancellationToken cancellationToken = new())
    {
        var query = _entities.AsQueryable();

        if (include is not null) query = include(query);

        var result = query.Where(predicate).Select(selector).FirstOrDefault();

        return Task.FromResult(result);
    }

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = new())
    {
        var result = predicate is null ? _entities.Count() : _entities.AsQueryable().Count(predicate);
        return Task.FromResult(result);
    }

    public Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = new())
    {
        var result = predicate is null ? _entities.Any() : _entities.AsQueryable().Any(predicate);
        return Task.FromResult(result);
    }

    public Task<bool> AllAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = new())
    {
        var result = _entities.AsQueryable().All(predicate);
        return Task.FromResult(result);
    }

    private T? GetById(Guid id) => _entities.FirstOrDefault(x => x.Id == id);
}