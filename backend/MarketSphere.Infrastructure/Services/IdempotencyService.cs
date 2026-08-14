using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Enums;
using MarketSphere.Infrastructure.Persistence;

namespace MarketSphere.Infrastructure.Services;

public sealed class IdempotencyService : IIdempotencyService
{
    private readonly MarketSphereDbContext _db;
    private readonly IDateTimeProvider _clock;

    public IdempotencyService(MarketSphereDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<IdempotencyBeginResult> BeginAsync(string idempotencyKey, int? userID, string endpoint, string requestHash, OfflineSyncContext? offlineSync, CancellationToken cancellationToken = default)
    {
        var key = idempotencyKey.Trim();
        var existing = await _db.IdempotencyRequests.SingleOrDefaultAsync(x => x.IdempotencyKey == key, cancellationToken);
        if (existing is not null)
        {
            if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
                return new IdempotencyBeginResult(existing.IdempotencyRequestID, false, true, 409, null, null);
            if (existing.ResponseStatusCode.HasValue)
                return new IdempotencyBeginResult(existing.IdempotencyRequestID, true, false, existing.ResponseStatusCode, existing.ResponseBody, null);
            return new IdempotencyBeginResult(existing.IdempotencyRequestID, false, true, 409, null, null);
        }

        var retentionHours = await GetRetentionHoursAsync(cancellationToken);
        var request = new IdempotencyRequest
        {
            IdempotencyKey = key,
            UserID = userID,
            Endpoint = endpoint,
            RequestHash = requestHash,
            CreatedAt = _clock.UtcNow,
            ExpiresAt = _clock.UtcNow.AddHours(retentionHours)
        };
        await _db.IdempotencyRequests.AddAsync(request, cancellationToken);

        OfflineSyncRecord? syncRecord = null;
        if (offlineSync is not null)
        {
            syncRecord = await _db.OfflineSyncRecords.SingleOrDefaultAsync(x => x.UserSessionID == offlineSync.UserSessionID && x.LocalRecordKey == offlineSync.LocalRecordKey, cancellationToken);
            if (syncRecord is null)
            {
                syncRecord = new OfflineSyncRecord
                {
                    UserSessionID = offlineSync.UserSessionID,
                    LocalRecordKey = offlineSync.LocalRecordKey,
                    EntityType = offlineSync.EntityType,
                    OperationType = Enum.IsDefined(typeof(OfflineOperationType), offlineSync.OperationType) ? (OfflineOperationType)offlineSync.OperationType : OfflineOperationType.Create,
                    PayloadJson = offlineSync.PayloadJson,
                    ClientTimestamp = offlineSync.ClientTimestamp,
                    SyncStatus = OfflineSyncStatus.Processing,
                    RetryCount = 0
                };
                await _db.OfflineSyncRecords.AddAsync(syncRecord, cancellationToken);
            }
            else if (syncRecord.SyncStatus == OfflineSyncStatus.Synced)
            {
                return new IdempotencyBeginResult(request.IdempotencyRequestID, false, true, 409, null, syncRecord.OfflineSyncRecordID);
            }
            else
            {
                syncRecord.SyncStatus = OfflineSyncStatus.Processing;
                syncRecord.RetryCount++;
                syncRecord.LastError = null;
                syncRecord.PayloadJson = offlineSync.PayloadJson;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return new IdempotencyBeginResult(request.IdempotencyRequestID, false, false, null, null, syncRecord?.OfflineSyncRecordID);
    }

    public async Task CompleteAsync(int idempotencyRequestID, int responseStatusCode, string? responseBody, int? offlineSyncRecordID, int? serverEntityID, CancellationToken cancellationToken = default)
    {
        var request = await _db.IdempotencyRequests.SingleAsync(x => x.IdempotencyRequestID == idempotencyRequestID, cancellationToken);
        request.ResponseStatusCode = responseStatusCode;
        request.ResponseBody = responseBody;
        if (offlineSyncRecordID.HasValue)
        {
            var record = await _db.OfflineSyncRecords.SingleAsync(x => x.OfflineSyncRecordID == offlineSyncRecordID, cancellationToken);
            record.SyncStatus = responseStatusCode is >= 200 and < 300 ? OfflineSyncStatus.Synced : OfflineSyncStatus.Failed;
            record.ServerTimestamp = _clock.UtcNow;
            record.ServerEntityID = serverEntityID;
            record.LastError = responseStatusCode is >= 200 and < 300 ? null : responseBody;
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task FailAsync(int idempotencyRequestID, int? offlineSyncRecordID, string error, CancellationToken cancellationToken = default)
    {
        if (offlineSyncRecordID.HasValue)
        {
            var record = await _db.OfflineSyncRecords.SingleOrDefaultAsync(x => x.OfflineSyncRecordID == offlineSyncRecordID, cancellationToken);
            if (record is not null) { record.SyncStatus = OfflineSyncStatus.Failed; record.ServerTimestamp = _clock.UtcNow; record.LastError = error.Length > 2000 ? error[..2000] : error; }
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> GetRetentionHoursAsync(CancellationToken cancellationToken)
    {
        var value = await _db.SystemSettings.AsNoTracking().Where(x => x.SettingKey == SystemSettingKeys.IdempotencyRetentionHours && x.ScopeType == SettingScopeType.Global).Select(x => x.SettingValue).FirstOrDefaultAsync(cancellationToken);
        return int.TryParse(value, out var hours) && hours > 0 ? hours : 24;
    }
}
