using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhlegmaticOne.UnitOfWork.Implementation;
using PhlegmaticOne.UnitOfWork.Interfaces;

namespace PhlegmaticOne.UnitOfWork.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection serviceCollection,
        Action<UnitOfWorkBuilder>? actionBuilder = null) where TContext : DbContext
    {
        if (actionBuilder is not null)
        {
            var builder = new UnitOfWorkBuilder(serviceCollection);
            actionBuilder(builder);
        }

        return serviceCollection.AddScoped<IUnitOfWork>(x =>
        {
            var dbContext = x.GetRequiredService<TContext>();
            var customRepositories = x.GetRequiredService<IEnumerable<IRepository>>();
            return new DbContextUnitOfWork(dbContext, customRepositories);
        });
    }
}