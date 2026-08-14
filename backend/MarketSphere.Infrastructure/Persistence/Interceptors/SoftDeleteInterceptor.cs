using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MarketSphere.Infrastructure.Persistence.Interceptors;

public sealed class SoftDeleteInterceptor :
    SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public SoftDeleteInterceptor(
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
        ConvertDeletes(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>>
        SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
    {
        ConvertDeletes(eventData.Context);
        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void ConvertDeletes(DbContext? context)
    {
        if (context is null)
            return;

        var now = _clock.UtcNow;
        var userID = _currentUser.UserID;

        foreach (var entry in context.ChangeTracker
                     .Entries<SoftDeletableEntity>()
                     .Where(x => x.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = now;
            entry.Entity.DeletedByUserID = userID;
            entry.Entity.UpdatedAt = now;
            entry.Entity.UpdatedByUserID = userID;
        }
    }
}
