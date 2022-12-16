using Microsoft.EntityFrameworkCore;
using PhlegmaticOne.UnitOfWork.Interfaces;
using PhlegmaticOne.UnitOfWork.Models;

namespace PhlegmaticOne.UnitOfWork.Implementation;

public class DbContextUnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly Dictionary<Type, IRepository> _repositories;

    public DbContextUnitOfWork(DbContext dbContext)
    {
        _repositories = new Dictionary<Type, IRepository>();
        _dbContext = dbContext;
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : EntityBase
    {
        var type = typeof(TEntity);

        if (_repositories.ContainsKey(type) == false)
        {
            var set = _dbContext.Set<TEntity>();
            _repositories[type] = new DbSetRepository<TEntity>(set);
        }

        return (IRepository<TEntity>)_repositories[type];
    }

    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}