using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Domain.Entities.Infrastructure;

namespace MarketSphere.Application.Modules.Infrastructure.Services;

public sealed class AuditService : IAuditService
{
    private static readonly string[] SensitiveNames = ["password", "token", "secret", "hash", "authorization"];
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public AuditService(IApplicationDbContext db, ICurrentUserService currentUser, IDateTimeProvider clock) { _db = db; _currentUser = currentUser; _clock = clock; }

    public Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.AuditLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search)) { var search = request.Search.Trim(); query = query.Where(x => x.ActionName.Contains(search) || x.EntityType.Contains(search)); }
        return InfrastructureServiceHelper.ToPagedAsync(query.OrderByDescending(x => x.CreatedAt).Select(x => new AuditLogDto(x.AuditLogID, x.UserID, x.ActionName, x.EntityType, x.EntityID, x.OldValuesJson, x.NewValuesJson, x.IPAddress, x.DeviceIdentifier, x.CreatedAt)), request, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StatusHistoryDto>> GetStatusHistoryAsync(string entityType, int entityID, CancellationToken cancellationToken = default)
    {
        var type = InfrastructureServiceHelper.Required(entityType, "Entity type", 100).ToUpperInvariant();
        return await _db.StatusHistories.AsNoTracking().Where(x => x.EntityType == type && x.EntityID == entityID).OrderBy(x => x.ChangedAt).Select(x => new StatusHistoryDto(x.StatusHistoryID, x.EntityType, x.EntityID, x.OldStatus, x.NewStatus, x.Reason, x.ChangedByUserID, x.ChangedAt)).ToListAsync(cancellationToken);
    }

    public async Task WriteAsync(WriteAuditRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = new AuditLog { UserID = _currentUser.UserID, ActionName = InfrastructureServiceHelper.Required(request.ActionName, "Action name", 150), EntityType = InfrastructureServiceHelper.Required(request.EntityType, "Entity type", 100).ToUpperInvariant(), EntityID = request.EntityID, OldValuesJson = SerializeMasked(request.OldValues), NewValuesJson = SerializeMasked(request.NewValues), IPAddress = request.IPAddress?.Trim(), DeviceIdentifier = request.DeviceIdentifier?.Trim(), CreatedAt = _clock.UtcNow };
        await _db.AddAsync(entity, cancellationToken); await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AppendStatusAsync(AppendStatusHistoryRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = new StatusHistory { EntityType = InfrastructureServiceHelper.Required(request.EntityType, "Entity type", 100).ToUpperInvariant(), EntityID = request.EntityID, OldStatus = request.OldStatus?.Trim(), NewStatus = InfrastructureServiceHelper.Required(request.NewStatus, "New status", 100), Reason = request.Reason?.Trim(), ChangedByUserID = _currentUser.UserID, ChangedAt = _clock.UtcNow };
        await _db.AddAsync(entity, cancellationToken); await _db.SaveChangesAsync(cancellationToken);
    }

    private static string? SerializeMasked(object? value)
    {
        if (value is null) return null;
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(value));
        return Mask(document.RootElement);
    }

    private static string Mask(JsonElement element)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream)) WriteElement(writer, element, null);
        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteElement(Utf8JsonWriter writer, JsonElement element, string? propertyName)
    {
        var sensitive = propertyName is not null && SensitiveNames.Any(x => propertyName.Contains(x, StringComparison.OrdinalIgnoreCase));
        if (sensitive) { writer.WriteStringValue("***MASKED***"); return; }
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject(); foreach (var property in element.EnumerateObject()) { writer.WritePropertyName(property.Name); WriteElement(writer, property.Value, property.Name); }
                writer.WriteEndObject(); break;
            case JsonValueKind.Array:
                writer.WriteStartArray(); foreach (var item in element.EnumerateArray()) WriteElement(writer, item, propertyName); writer.WriteEndArray(); break;
            default: element.WriteTo(writer); break;
        }
    }
}
