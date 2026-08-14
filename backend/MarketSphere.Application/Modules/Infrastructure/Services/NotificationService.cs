using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.Infrastructure.Services;

public sealed class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public NotificationService(IApplicationDbContext db, ICurrentUserService currentUser, IDateTimeProvider clock) { _db = db; _currentUser = currentUser; _clock = clock; }

    public Task<PagedResult<NotificationDto>> GetMineAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var userID = RequireUser();
        var query = _db.Notifications.AsNoTracking().Where(x => x.UserID == userID && (!x.ExpiresAt.HasValue || x.ExpiresAt > _clock.UtcNow));
        if (!string.IsNullOrWhiteSpace(request.Search)) { var search = request.Search.Trim(); query = query.Where(x => x.Title.Contains(search) || x.Message.Contains(search)); }
        return InfrastructureServiceHelper.ToPagedAsync(query.OrderBy(x => x.IsRead).ThenByDescending(x => x.Priority).ThenByDescending(x => x.CreatedAt).Select(ToDto()), request, cancellationToken);
    }

    public async Task<int> CreateAsync(CreateNotificationRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!await _db.Users.AnyAsync(x => x.UserID == request.UserID, cancellationToken)) throw new NotFoundException("User was not found.");
        var entity = new Notification { UserID = request.UserID, NotificationType = request.NotificationType, Title = InfrastructureServiceHelper.Required(request.Title, "Notification title", 200), Message = InfrastructureServiceHelper.Required(request.Message, "Notification message", 2000), Priority = request.Priority, ReferenceType = request.ReferenceType?.Trim().ToUpperInvariant(), ReferenceID = request.ReferenceID, CreatedAt = _clock.UtcNow, ExpiresAt = request.ExpiresAt };
        await _db.AddAsync(entity, cancellationToken); await _db.SaveChangesAsync(cancellationToken); return entity.NotificationID;
    }

    public async Task MarkReadAsync(int id, CancellationToken cancellationToken = default)
    {
        var userID = RequireUser(); var entity = await InfrastructureServiceHelper.RequireAsync(_db.Notifications.Where(x => x.NotificationID == id && x.UserID == userID), "Notification", cancellationToken); if (!entity.IsRead) { entity.IsRead = true; entity.ReadAt = _clock.UtcNow; await _db.SaveChangesAsync(cancellationToken); }
    }

    public async Task MarkAllReadAsync(CancellationToken cancellationToken = default)
    {
        var userID = RequireUser(); var items = await _db.Notifications.Where(x => x.UserID == userID && !x.IsRead).ToListAsync(cancellationToken); foreach (var item in items) { item.IsRead = true; item.ReadAt = _clock.UtcNow; }
        if (items.Count > 0) await _db.SaveChangesAsync(cancellationToken);
    }

    private int RequireUser() => _currentUser.UserID ?? throw new ForbiddenBusinessActionException("Authenticated user is required.");
    private static System.Linq.Expressions.Expression<Func<Notification, NotificationDto>> ToDto() => x => new NotificationDto(x.NotificationID, x.NotificationType, x.Title, x.Message, x.Priority, x.ReferenceType, x.ReferenceID, x.IsRead, x.CreatedAt, x.ExpiresAt, x.ReadAt);
}
