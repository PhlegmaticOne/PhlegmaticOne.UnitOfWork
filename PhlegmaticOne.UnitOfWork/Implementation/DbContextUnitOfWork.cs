﻿using Microsoft.EntityFrameworkCore;
using PhlegmaticOne.DomainDefinitions;
using PhlegmaticOne.UnitOfWork.Interfaces;

namespace PhlegmaticOne.UnitOfWork.Implementation;

public class DbContextUnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly Dictionary<Type, IRepository> _repositories;
    private readonly Dictionary<Type, IRepository> _customRepositories;
    public DbContextUnitOfWork(DbContext dbContext, IEnumerable<IRepository> customRepositories)
    {
        _repositories = new Dictionary<Type, IRepository>();
        _dbContext = dbContext;
        _customRepositories = customRepositories.ToDictionary(x => x.GetType(), x => x);
    }

    public TRepository GetCustomRepository<TRepository>() where TRepository : IRepository => 
        (TRepository)_customRepositories[typeof(TRepository)];

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity
    {
        var type = typeof(TEntity);

        if (_repositories.ContainsKey(type) == false)
        {
            var set = _dbContext.Set<TEntity>();
            _repositories[type] = new DbSetRepository<TEntity>(set);
        }

        return (IRepository<TEntity>)_repositories[type];
    }

    public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();
}