using Microsoft.EntityFrameworkCore;
using PhlegmaticOne.DomainDefinitions.Objects;
using PhlegmaticOne.UnitOfWork.Abstractions;
using PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Base;

namespace PhlegmaticOne.UnitOfWork.Implementation.Implementation;

public class DbContextUnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly Dictionary<Type, IRepository> _repositories;
    private readonly Dictionary<Type, IRepository> _customRepositories;
    private readonly IEnumerable<IUnitOfWorkInterceptor> _interceptors;
    public DbContextUnitOfWork(DbContext dbContext, 
        IEnumerable<IRepository> customRepositories,
        IEnumerable<IUnitOfWorkInterceptor> interceptors)
    {
        _repositories = new Dictionary<Type, IRepository>();
        _dbContext = dbContext;
        _interceptors = interceptors;
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

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.ProcessAsync(_dbContext, cancellationToken);
        }
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}