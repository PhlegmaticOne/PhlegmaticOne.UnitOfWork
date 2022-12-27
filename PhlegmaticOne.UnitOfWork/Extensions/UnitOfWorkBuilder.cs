using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PhlegmaticOne.UnitOfWork.Interfaces;

namespace PhlegmaticOne.UnitOfWork.Extensions;

public class UnitOfWorkBuilder
{
    private readonly IServiceCollection _serviceCollection;

    public UnitOfWorkBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void AddCustomRepositoriesFromAssembly(Assembly assembly)
    {
        var repositoryType = typeof(IRepository);
        var customRepositoryTypes = assembly
            .GetTypes()
            .Where(x => x.IsAbstract == false && x.IsAssignableTo(repositoryType));

        foreach (var customRepositoryType in customRepositoryTypes)
        {
            _serviceCollection.AddScoped(repositoryType, customRepositoryType);
        }
    }
}