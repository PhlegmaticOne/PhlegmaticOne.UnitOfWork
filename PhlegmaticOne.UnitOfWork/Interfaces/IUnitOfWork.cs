using PhlegmaticOne.UnitOfWork.Models;

namespace PhlegmaticOne.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : EntityBase;
    Task<int> SaveChangesAsync();
}