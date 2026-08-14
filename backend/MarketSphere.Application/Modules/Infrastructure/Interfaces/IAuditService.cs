using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Infrastructure.DTOs;

namespace MarketSphere.Application.Modules.Infrastructure.Interfaces;

public interface IAuditService
{
    Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<StatusHistoryDto>> GetStatusHistoryAsync(string entityType, int entityID, CancellationToken cancellationToken = default);
    Task WriteAsync(WriteAuditRequestDto request, CancellationToken cancellationToken = default);
    Task AppendStatusAsync(AppendStatusHistoryRequestDto request, CancellationToken cancellationToken = default);
}
