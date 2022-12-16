using PhlegmaticOne.UnitOfWork.Interfaces;
using PhlegmaticOne.UnitOfWork.Models;

namespace PhlegmaticOne.UnitOfWork.Implementation;

public class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly Dictionary<Type, IRepository> _repositories;
    public InMemoryUnitOfWork() => _repositories = new Dictionary<Type, IRepository>();

    public InMemoryUnitOfWork(IEnumerable<IRepository> repositories) =>
        _repositories = repositories.ToDictionary(
            key => key.GetType().GetGenericArguments().First(),
            value => value);

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : EntityBase
    {
        var type = typeof(TEntity);

        if (_repositories.ContainsKey(type) == false)
        {
            _repositories[type] = new InMemoryRepository<TEntity>();
        }

        return (IRepository<TEntity>)_repositories[type];
    }

    public Task<int> SaveChangesAsync() => Task.FromResult(0);
}