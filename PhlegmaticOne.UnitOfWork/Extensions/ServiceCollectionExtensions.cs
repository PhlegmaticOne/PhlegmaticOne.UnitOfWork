using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhlegmaticOne.UnitOfWork.Implementation;
using PhlegmaticOne.UnitOfWork.Interfaces;

namespace PhlegmaticOne.UnitOfWork.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection serviceCollection)
        where TContext : DbContext
    {
        serviceCollection.AddScoped<IUnitOfWork>(x =>
        {
            var dbContext = x.GetRequiredService<TContext>();
            return new DbContextUnitOfWork(dbContext);
        });
        return serviceCollection;
    }
}