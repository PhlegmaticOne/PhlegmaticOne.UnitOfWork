using PhlegmaticOne.DomainDefinitions;

namespace PhlegmaticOne.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    TRepository GetCustomRepository<TRepository>() where TRepository : IRepository;
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity;
    Task<int> SaveChangesAsync();
}