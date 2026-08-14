using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;

public interface IRoleService
{
    Task<IReadOnlyCollection<RoleListItemDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<RoleDetailsDto> GetByIDAsync(
        int roleID,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        CreateRoleRequestDto request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        int roleID,
        UpdateRoleRequestDto request,
        CancellationToken cancellationToken = default);

    Task UpdatePermissionsAsync(
        int roleID,
        UpdateRolePermissionsRequestDto request,
        CancellationToken cancellationToken = default);
}
