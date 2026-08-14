using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MarketSphere.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor :
    SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public AuditableEntityInterceptor(
        ICurrentUserService currentUser,
        IDateTimeProvider clock)
    {
        _currentUser = currentUser;
        _clock = clock;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditValues(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>>
        SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
    {
        ApplyAuditValues(eventData.Context);
        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void ApplyAuditValues(DbContext? context)
    {
        if (context is null)
            return;

        var now = _clock.UtcNow;
        var userID = _currentUser.UserID;

        foreach (var entry in context.ChangeTracker
                     .Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedAt == default)
                    entry.Entity.CreatedAt = now;

                entry.Entity.CreatedByUserID ??= userID;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.CreatedAt)
                    .IsModified = false;

                entry.Property(x => x.CreatedByUserID)
                    .IsModified = false;

                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedByUserID = userID;
            }
        }
    }
}
