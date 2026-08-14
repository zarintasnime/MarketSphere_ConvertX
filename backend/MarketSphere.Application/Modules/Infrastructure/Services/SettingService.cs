using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.Infrastructure.Services;

public sealed class SettingService : ISettingService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SettingService(IApplicationDbContext db, ICurrentUserService currentUser) { _db = db; _currentUser = currentUser; }

    public async Task<IReadOnlyCollection<SystemSettingDto>> GetAsync(CancellationToken cancellationToken = default)
        => await _db.SystemSettings.AsNoTracking().OrderBy(x => x.SettingKey).ThenBy(x => x.ScopeType).Select(ToDto()).ToListAsync(cancellationToken);

    public async Task<SystemSettingDto?> GetByKeyAsync(string key, int? scopeID = null, CancellationToken cancellationToken = default)
    {
        var normalized = InfrastructureServiceHelper.Required(key, "Setting key", 200);
        return await _db.SystemSettings.AsNoTracking().Where(x => x.SettingKey == normalized && (x.ScopeID == scopeID || x.ScopeType == SettingScopeType.Global)).OrderByDescending(x => x.ScopeID == scopeID).Select(ToDto()).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> SaveAsync(int? id, SaveSystemSettingRequestDto request, CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.UserID ?? throw new ForbiddenBusinessActionException("Authenticated user is required.");
        var key = InfrastructureServiceHelper.Required(request.SettingKey, "Setting key", 200);
        var value = InfrastructureServiceHelper.Required(request.SettingValue, "Setting value", 4000);
        ValidateValue(value, request.DataType);
        if (request.ScopeType == SettingScopeType.Global && request.ScopeID.HasValue) throw new BusinessRuleException("A global setting cannot have a scope ID.");
        if (request.ScopeType != SettingScopeType.Global && !request.ScopeID.HasValue) throw new BusinessRuleException("A non-global setting requires a scope ID.");
        if (await _db.SystemSettings.AnyAsync(x => x.SystemSettingID != id && x.SettingKey == key && x.ScopeType == request.ScopeType && x.ScopeID == request.ScopeID, cancellationToken)) throw new ConflictException("The setting key already exists in the same scope.");
        SystemSetting entity;
        if (id.HasValue) entity = await InfrastructureServiceHelper.RequireAsync(_db.SystemSettings.Where(x => x.SystemSettingID == id), "System setting", cancellationToken);
        else { entity = new SystemSetting(); await _db.AddAsync(entity, cancellationToken); }
        entity.SettingKey = key; entity.SettingValue = value; entity.DataType = request.DataType; entity.ScopeType = request.ScopeType; entity.ScopeID = request.ScopeID; entity.Description = request.Description?.Trim(); entity.IsEncrypted = request.IsEncrypted; entity.UpdatedByUserID = userID; await _db.SaveChangesAsync(cancellationToken); return entity.SystemSettingID;
    }

    private static void ValidateValue(string value, SettingDataType dataType)
    {
        var valid = dataType switch { SettingDataType.String => true, SettingDataType.Integer => int.TryParse(value, out _), SettingDataType.Decimal => decimal.TryParse(value, out _), SettingDataType.Boolean => bool.TryParse(value, out _), SettingDataType.DateTime => DateTime.TryParse(value, out _), SettingDataType.Json => IsJson(value), _ => false };
        if (!valid) throw new BusinessRuleException("Setting value does not match the selected data type.");
    }
    private static bool IsJson(string value) { try { System.Text.Json.JsonDocument.Parse(value).Dispose(); return true; } catch { return false; } }
    private static System.Linq.Expressions.Expression<Func<SystemSetting, SystemSettingDto>> ToDto() => x => new SystemSettingDto(x.SystemSettingID, x.SettingKey, x.SettingValue, x.DataType, x.ScopeType, x.ScopeID, x.Description, x.IsEncrypted, x.UpdatedByUserID, x.UpdatedAt);
}
