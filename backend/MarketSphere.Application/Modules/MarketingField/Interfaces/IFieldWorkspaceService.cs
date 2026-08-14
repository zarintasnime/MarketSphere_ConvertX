using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;

namespace MarketSphere.Application.Modules.MarketingField.Interfaces;

public interface IFieldWorkspaceService
{
    Task<FieldWorkspaceSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<PagedResult<FieldAssignedClientDto>> GetAssignedClientsAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<PagedResult<FieldVisitListDto>> GetMyVisitsAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<FieldActiveVisitDto?> GetActiveVisitAsync(
        CancellationToken cancellationToken = default);
}
