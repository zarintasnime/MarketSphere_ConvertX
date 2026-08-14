namespace MarketSphere.Application.Common.Models;

public sealed record OfflineSyncContext(
    int UserSessionID,
    string LocalRecordKey,
    string EntityType,
    int OperationType,
    string PayloadJson,
    DateTime ClientTimestamp);

public sealed record IdempotencyBeginResult(
    int IdempotencyRequestID,
    bool IsReplay,
    bool IsConflict,
    int? ResponseStatusCode,
    string? ResponseBody,
    int? OfflineSyncRecordID);
