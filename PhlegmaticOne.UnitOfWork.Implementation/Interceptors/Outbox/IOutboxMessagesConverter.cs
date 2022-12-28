using PhlegmaticOne.DomainDefinitions.Events;
using PhlegmaticOne.UnitOfWork.Implementation.Outbox;

namespace PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Outbox;

public interface IOutboxMessagesConverter<out T> where T : OutboxMessage
{
    IEnumerable<T> ConvertDomainEventsToOutboxMessages(IEnumerable<IDomainEvent> domainEvents);
}