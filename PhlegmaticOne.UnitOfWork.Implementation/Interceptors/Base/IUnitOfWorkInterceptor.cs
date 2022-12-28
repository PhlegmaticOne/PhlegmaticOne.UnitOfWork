using Microsoft.EntityFrameworkCore;

namespace PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Base;

public interface IUnitOfWorkInterceptor
{
    Task ProcessAsync(DbContext dbContext, CancellationToken cancellationToken = default);
}