using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserListItemDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<UserDetailsDto> GetByIDAsync(
        int userID,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        CreateUserRequestDto request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        int userID,
        UpdateUserRequestDto request,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        int userID,
        ChangeUserStatusRequestDto request,
        CancellationToken cancellationToken = default);

    Task AssignRolesAsync(
        int userID,
        AssignUserRolesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<AccountTokenResultDto> CreateAccountTokenAsync(
        int userID,
        AccountTokenType tokenType,
        CancellationToken cancellationToken = default);
}
