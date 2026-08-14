using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Entities.Infrastructure;

namespace MarketSphere.Infrastructure.Persistence.Interceptors;

public sealed class StatusHistoryInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public StatusHistoryInterceptor(ICurrentUserService currentUser, IDateTimeProvider clock)
    {
        _currentUser = currentUser;
        _clock = clock;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AppendHistory(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AppendHistory(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AppendHistory(DbContext? context)
    {
        if (context is null) return;
        var entries = context.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified && x.Metadata.FindProperty("Status") is not null && x.Entity is not StatusHistory && x.Entity is not AuditLog).ToArray();
        foreach (var entry in entries)
        {
            var status = entry.Property("Status");
            if (!status.IsModified || Equals(status.OriginalValue, status.CurrentValue)) continue;
            var key = GetPrimaryKey(entry);
            if (!key.HasValue || key.Value <= 0) continue;
            context.Set<StatusHistory>().Add(new StatusHistory
            {
                EntityType = entry.Metadata.ClrType.Name.ToUpperInvariant(),
                EntityID = key.Value,
                OldStatus = status.OriginalValue?.ToString(),
                NewStatus = status.CurrentValue?.ToString() ?? string.Empty,
                ChangedByUserID = _currentUser.UserID,
                ChangedAt = _clock.UtcNow
            });
        }
    }

    private static int? GetPrimaryKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey()?.Properties.SingleOrDefault();
        if (key is null) return null;
        var value = entry.Property(key.Name).CurrentValue;
        return value is int intValue ? intValue : null;
    }
}
