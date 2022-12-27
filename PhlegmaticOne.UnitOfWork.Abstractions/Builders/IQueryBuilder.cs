namespace PhlegmaticOne.UnitOfWork.Abstractions.Builders;

public interface IQueryBuilder<out TEntity> where TEntity : class
{
    IQueryable<TEntity> Build();
}