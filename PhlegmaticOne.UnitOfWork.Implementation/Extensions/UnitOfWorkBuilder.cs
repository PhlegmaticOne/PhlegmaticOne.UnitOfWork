using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PhlegmaticOne.UnitOfWork.Abstractions;
using PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Base;
using PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Outbox;
using PhlegmaticOne.UnitOfWork.Implementation.Outbox;

namespace PhlegmaticOne.UnitOfWork.Implementation.Extensions;

public class UnitOfWorkBuilder
{
    private readonly IServiceCollection _serviceCollection;

    public UnitOfWorkBuilder(IServiceCollection serviceCollection) => _serviceCollection = serviceCollection;

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

    public void AddInterceptorsFromAssembly(Assembly assembly)
    {
        var interceptorsType = typeof(IUnitOfWorkInterceptor);
        var interceptorTypes = assembly
            .GetTypes()
            .Where(x => x.IsAbstract == false && x.IsAssignableTo(interceptorsType));

        foreach (var interceptorType in interceptorTypes)
        {
            _serviceCollection.AddScoped(interceptorsType, interceptorType);
        }
    }

    public void AddInterceptor<TInterceptor>() where TInterceptor : class, IUnitOfWorkInterceptor
    {
        _serviceCollection.AddScoped<IUnitOfWorkInterceptor, TInterceptor>();
    }

    public OutboxMessagesInterceptorBuilder<TMessage> AddOutboxMessagesInterceptor<TMessage>() 
        where TMessage : OutboxMessage
    {
        _serviceCollection.AddScoped<IUnitOfWorkInterceptor, OutboxMessagesInterceptor<TMessage>>();
        return new OutboxMessagesInterceptorBuilder<TMessage>(_serviceCollection);
    }

    public void AddDefaultOutboxMessagesInterceptor()
    {
        AddOutboxMessagesInterceptor<OutboxMessage>()
            .UsingDomainEventsToOutboxMessageConverter<DefaultOutboxMessagesConverter>();
    }
}

public class OutboxMessagesInterceptorBuilder<TMessage> where TMessage : OutboxMessage
{
    private readonly IServiceCollection _serviceCollection;

    public OutboxMessagesInterceptorBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void UsingDomainEventsToOutboxMessageConverter<TConverter>()
        where TConverter : class, IOutboxMessagesConverter<TMessage>
    {
        _serviceCollection.AddSingleton<IOutboxMessagesConverter<TMessage>, TConverter>();
    }
}