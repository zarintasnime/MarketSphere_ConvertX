using MarketSphere.Application.Common.Models;

namespace MarketSphere.Application.Common.Interfaces;

public interface IIdempotencyService
{
    Task<IdempotencyBeginResult> BeginAsync(
        string idempotencyKey,
        int? userID,
        string endpoint,
        string requestHash,
        OfflineSyncContext? offlineSync,
        CancellationToken cancellationToken = default);

    Task CompleteAsync(
        int idempotencyRequestID,
        int responseStatusCode,
        string? responseBody,
        int? offlineSyncRecordID,
        int? serverEntityID,
        CancellationToken cancellationToken = default);

    Task FailAsync(
        int idempotencyRequestID,
        int? offlineSyncRecordID,
        string error,
        CancellationToken cancellationToken = default);
}
