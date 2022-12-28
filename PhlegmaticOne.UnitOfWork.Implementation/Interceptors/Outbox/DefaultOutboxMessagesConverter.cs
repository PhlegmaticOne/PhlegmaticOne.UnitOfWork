using Newtonsoft.Json;
using PhlegmaticOne.DomainDefinitions.Events;
using PhlegmaticOne.UnitOfWork.Implementation.Outbox;

namespace PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Outbox;

internal class DefaultOutboxMessagesConverter : IOutboxMessagesConverter<OutboxMessage>
{
    public IEnumerable<OutboxMessage> ConvertDomainEventsToOutboxMessages(IEnumerable<IDomainEvent> domainEvents)
    {
        return domainEvents.Select(x => new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredAtUtc = DateTime.UtcNow,
            Type = x.GetType().Name,
            Content = JsonConvert.SerializeObject(x, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            })
        });
    }
}