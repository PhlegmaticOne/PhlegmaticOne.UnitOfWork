using PhlegmaticOne.DomainDefinitions.Objects;

namespace PhlegmaticOne.UnitOfWork.Abstractions;

public interface IUnitOfWork
{
    TRepository GetCustomRepository<TRepository>() where TRepository : IRepository;
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}