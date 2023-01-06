using Microsoft.EntityFrameworkCore;
using PhlegmaticOne.DomainDefinitions.Base;
using PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Base;

namespace PhlegmaticOne.UnitOfWork.Implementation.Interceptors.Auditable;

public class AuditableUnitOfWorkInterceptor : IUnitOfWorkInterceptor
{
    public Task ProcessAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        var entries = dbContext.ChangeTracker.Entries<IAuditable>();

        foreach (var entry in entries)
        {
            var propertyName = GetPropertyName(entry.State);

            if (propertyName is not null)
            {
                entry.Property(propertyName).CurrentValue = DateTime.UtcNow;
            }
        }

        return Task.CompletedTask;
    }

    private static string? GetPropertyName(EntityState entityState) =>
        entityState switch
        {
            EntityState.Added => nameof(IAuditable.CreatedAtUtc),
            EntityState.Modified => nameof(IAuditable.ModifiedAtUtc),
            _ => null
        };
}