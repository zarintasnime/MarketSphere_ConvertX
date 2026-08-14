using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Enums;
using MarketSphere.Infrastructure.Persistence;

namespace MarketSphere.Infrastructure.Services;

public sealed class SystemCheckService : ISystemCheckService
{
    private readonly MarketSphereDbContext _db;
    private readonly IDateTimeProvider _clock;
    public SystemCheckService(MarketSphereDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }

    public async Task<IReadOnlyCollection<SystemCheckResult>> RunAsync(CancellationToken cancellationToken = default)
    {
        var now = _clock.UtcNow; var results = new List<SystemCheckResult>();
        results.Add(await CheckTasksAsync(now, cancellationToken));
        results.Add(await CheckQuotationsAsync(now, await IntSettingAsync(SystemSettingKeys.QuotationExpiryAlertDays, 7, cancellationToken), cancellationToken));
        results.Add(await CheckComplaintsAsync(now, cancellationToken));
        results.Add(await CheckBatchesAsync(now, await IntSettingAsync(SystemSettingKeys.NearExpiryAlertDays, 30, cancellationToken), cancellationToken));
        results.Add(await CheckInactiveClientsAsync(now, await IntSettingAsync(SystemSettingKeys.InactiveClientDays, 90, cancellationToken), cancellationToken));
        return results;
    }

    private async Task<SystemCheckResult> CheckTasksAsync(DateTime now, CancellationToken ct)
    {
        var rows = await _db.CRMTasks.AsNoTracking().Where(x => x.DueAt < now && x.Status != CrmTaskStatus.Completed && x.Status != CrmTaskStatus.Cancelled && x.AssignedEmployee.UserID.HasValue).Select(x => new { ID = x.CRMTaskID, UserID = x.AssignedEmployee.UserID!.Value, x.Title }).ToListAsync(ct); var created = 0;
        foreach (var row in rows) created += await EnsureNotificationAsync(row.UserID, "Overdue CRM task", $"Task '{row.Title}' is overdue.", NotificationType.ActionRequired, NotificationPriority.High, "CRM_TASK", row.ID, ct);
        return new SystemCheckResult("OverdueTask", rows.Count, created);
    }

    private async Task<SystemCheckResult> CheckQuotationsAsync(DateTime now, int days, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(now); var until = today.AddDays(days); var rows = await _db.Quotations.AsNoTracking().Where(x => x.ValidUntil >= today && x.ValidUntil <= until && (x.Status == QuotationStatus.Submitted || x.Status == QuotationStatus.Reviewed) && x.Opportunity != null && x.Opportunity.OwnerEmployee.UserID.HasValue).Select(x => new { ID = x.QuotationID, UserID = x.Opportunity!.OwnerEmployee.UserID!.Value, x.QuotationNo, x.ValidUntil }).ToListAsync(ct); var created = 0;
        foreach (var row in rows) created += await EnsureNotificationAsync(row.UserID, "Quotation expiring", $"Quotation {row.QuotationNo} expires on {row.ValidUntil:yyyy-MM-dd}.", NotificationType.Expiry, NotificationPriority.High, ReferenceTypeCodes.Quotation, row.ID, ct);
        return new SystemCheckResult("ExpiringQuotation", rows.Count, created);
    }

    private async Task<SystemCheckResult> CheckComplaintsAsync(DateTime now, CancellationToken ct)
    {
        var rows = await _db.Complaints.AsNoTracking().Where(x => x.SLADueAt.HasValue && x.SLADueAt < now && x.Status != ComplaintStatus.Resolved && x.Status != ComplaintStatus.Closed && x.AssignedEmployee != null && x.AssignedEmployee.UserID.HasValue).Select(x => new { ID = x.ComplaintID, UserID = x.AssignedEmployee!.UserID!.Value, x.ComplaintNo }).ToListAsync(ct); var created = 0;
        foreach (var row in rows) created += await EnsureNotificationAsync(row.UserID, "Complaint SLA breached", $"Complaint {row.ComplaintNo} has exceeded its SLA.", NotificationType.Sla, NotificationPriority.Critical, ReferenceTypeCodes.Complaint, row.ID, ct);
        return new SystemCheckResult("ComplaintSLA", rows.Count, created);
    }

    private async Task<SystemCheckResult> CheckBatchesAsync(DateTime now, int days, CancellationToken ct)
    {
        var until = now.AddDays(days); var rows = await _db.Batches.AsNoTracking().Where(x => x.ExpiryDate.HasValue && x.ExpiryDate >= now && x.ExpiryDate <= until && x.Status == BatchStatus.Available).Select(x => new { ID = x.BatchID, x.BatchNo, x.ExpiryDate }).ToListAsync(ct); var users = await UsersInRoleAsync(RoleCodes.WarehouseOfficer, ct); var created = 0;
        foreach (var row in rows) foreach (var userID in users) created += await EnsureNotificationAsync(userID, "Batch near expiry", $"Batch {row.BatchNo} expires on {row.ExpiryDate:yyyy-MM-dd}.", NotificationType.Expiry, NotificationPriority.High, "BATCH", row.ID, ct);
        return new SystemCheckResult("NearExpiryBatch", rows.Count, created);
    }

    private async Task<SystemCheckResult> CheckInactiveClientsAsync(DateTime now, int days, CancellationToken ct)
    {
        var threshold = now.AddDays(-days); var rows = await _db.Clients.AsNoTracking().Where(x => x.IsActive && (!x.LastOrderAt.HasValue || x.LastOrderAt < threshold)).Select(x => new { ID = x.ClientID, x.ClientCode, x.ClientName }).ToListAsync(ct); var users = await UsersInRoleAsync(RoleCodes.CrmManager, ct); var created = 0;
        foreach (var row in rows) foreach (var userID in users) created += await EnsureNotificationAsync(userID, "Inactive client", $"Client {row.ClientCode} - {row.ClientName} is inactive.", NotificationType.Warning, NotificationPriority.Normal, ReferenceTypeCodes.Client, row.ID, ct);
        return new SystemCheckResult("InactiveClient", rows.Count, created);
    }

    private async Task<int> EnsureNotificationAsync(int userID, string title, string message, NotificationType type, NotificationPriority priority, string referenceType, int referenceID, CancellationToken ct)
    {
        var exists = await _db.Notifications.AnyAsync(x => x.UserID == userID && x.Title == title && x.ReferenceType == referenceType && x.ReferenceID == referenceID && !x.IsRead && (!x.ExpiresAt.HasValue || x.ExpiresAt > _clock.UtcNow), ct); if (exists) return 0;
        await _db.Notifications.AddAsync(new Notification { UserID = userID, Title = title, Message = message, NotificationType = type, Priority = priority, ReferenceType = referenceType, ReferenceID = referenceID, CreatedAt = _clock.UtcNow }, ct); await _db.SaveChangesAsync(ct); return 1;
    }

    private async Task<int[]> UsersInRoleAsync(string roleCode, CancellationToken ct) => await _db.UserRoles.AsNoTracking().Where(x => x.Role.RoleCode == roleCode && x.User.Status == UserStatus.Active).Select(x => x.UserID).Distinct().ToArrayAsync(ct);
    private async Task<int> IntSettingAsync(string key, int fallback, CancellationToken ct) { var value = await _db.SystemSettings.AsNoTracking().Where(x => x.SettingKey == key && x.ScopeType == SettingScopeType.Global).Select(x => x.SettingValue).FirstOrDefaultAsync(ct); return int.TryParse(value, out var result) ? result : fallback; }
}
