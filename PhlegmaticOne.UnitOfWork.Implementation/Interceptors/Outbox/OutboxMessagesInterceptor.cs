using Microsoft.EntityFrameworkCore;
using PhlegmaticOne.DomainDefinitions.Objects;
using PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Base;
using PhlegmaticOne.UnitOfWork.Implementation.Outbox;

namespace PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Outbox;

internal class OutboxMessagesInterceptor<T> : IUnitOfWorkInterceptor where T : OutboxMessage
{
    private readonly IOutboxMessagesConverter<T> _outboxMessagesConverter;

    public OutboxMessagesInterceptor(IOutboxMessagesConverter<T> outboxMessagesConverter) => 
        _outboxMessagesConverter = outboxMessagesConverter;

    public async Task ProcessAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        var domainEvents = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(x =>
            {
                var domainEvents = x.GetDomainEvents();
                x.ClearDomainEvents();
                return domainEvents;
            });

        var outboxMessages = _outboxMessagesConverter.ConvertDomainEventsToOutboxMessages(domainEvents);

        await dbContext.Set<T>().AddRangeAsync(outboxMessages, cancellationToken);
    }
}