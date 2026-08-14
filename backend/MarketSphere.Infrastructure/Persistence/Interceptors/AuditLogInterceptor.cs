using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Entities.Infrastructure;

namespace MarketSphere.Infrastructure.Persistence.Interceptors;

public sealed class AuditLogInterceptor : SaveChangesInterceptor
{
    private static readonly string[] SensitiveFragments = ["Password", "Token", "Secret", "Hash"];
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogInterceptor(ICurrentUserService currentUser, IDateTimeProvider clock, IHttpContextAccessor httpContextAccessor)
    {
        _currentUser = currentUser;
        _clock = clock;
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AppendAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AppendAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AppendAuditLogs(DbContext? context)
    {
        if (context is null) return;
        var entries = context.ChangeTracker.Entries().Where(x => (x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted) && x.Entity is not AuditLog && x.Entity is not StatusHistory && x.Entity is not Notification && x.Entity is not IdempotencyRequest && x.Entity is not OfflineSyncRecord).ToArray();
        foreach (var entry in entries)
        {
            var oldValues = entry.State == EntityState.Added ? null : Values(entry, true);
            var newValues = entry.State == EntityState.Deleted ? null : Values(entry, false);
            context.Set<AuditLog>().Add(new AuditLog
            {
                UserID = _currentUser.UserID,
                ActionName = entry.State.ToString().ToUpperInvariant(),
                EntityType = entry.Metadata.ClrType.Name.ToUpperInvariant(),
                EntityID = GetPrimaryKey(entry),
                OldValuesJson = oldValues,
                NewValuesJson = newValues,
                IPAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                DeviceIdentifier = _httpContextAccessor.HttpContext?.Request.Headers["X-Device-Identifier"].FirstOrDefault(),
                CreatedAt = _clock.UtcNow
            });
        }
    }

    private static string Values(EntityEntry entry, bool original)
    {
        var values = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            if (entry.State == EntityState.Modified && !property.IsModified) continue;
            var name = property.Metadata.Name;
            values[name] = SensitiveFragments.Any(x => name.Contains(x, StringComparison.OrdinalIgnoreCase)) ? "***MASKED***" : original ? property.OriginalValue : property.CurrentValue;
        }
        return JsonSerializer.Serialize(values);
    }

    private static int? GetPrimaryKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey()?.Properties.SingleOrDefault();
        if (key is null) return null;
        var value = entry.Property(key.Name).CurrentValue;
        return value is int intValue && intValue > 0 ? intValue : null;
    }
}
