using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class SystemSettingSeeder
{
    private readonly MarketSphereDbContext _db; private readonly IDateTimeProvider _clock;
    public SystemSettingSeeder(MarketSphereDbContext db, IDateTimeProvider clock) { _db = db; _clock = clock; }
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var userID = await _db.Users.OrderBy(x => x.UserID).Select(x => x.UserID).FirstAsync(cancellationToken);
        var values = new[] { (SystemSettingKeys.InactiveClientDays, "90", SettingDataType.Integer, "Days without an order before a client is considered inactive."), (SystemSettingKeys.QuotationExpiryAlertDays, "7", SettingDataType.Integer, "Days before quotation expiry to create an alert."), (SystemSettingKeys.ComplaintDefaultSlaHours, "48", SettingDataType.Integer, "Default complaint SLA hours."), (SystemSettingKeys.NearExpiryAlertDays, "30", SettingDataType.Integer, "Days before batch expiry to create an alert."), (SystemSettingKeys.OfferConflictMode, "MOST_SPECIFIC", SettingDataType.String, "Campaign and standard offer conflict mode."), (SystemSettingKeys.OfflineRetryLimit, "5", SettingDataType.Integer, "Maximum visible offline synchronization retry count."), (SystemSettingKeys.IdempotencyRetentionHours, "24", SettingDataType.Integer, "Idempotency response retention hours."), (SystemSettingKeys.NotificationRetentionDays, "90", SettingDataType.Integer, "Read notification retention days.") };
        foreach (var item in values) { var entity = await _db.SystemSettings.SingleOrDefaultAsync(x => x.SettingKey == item.Item1 && x.ScopeType == SettingScopeType.Global && x.ScopeID == null, cancellationToken); if (entity is null) await _db.SystemSettings.AddAsync(new SystemSetting { SettingKey = item.Item1, SettingValue = item.Item2, DataType = item.Item3, ScopeType = SettingScopeType.Global, Description = item.Item4, UpdatedByUserID = userID, CreatedAt = _clock.UtcNow }, cancellationToken); }
        await _db.SaveChangesAsync(cancellationToken);
    }
}
