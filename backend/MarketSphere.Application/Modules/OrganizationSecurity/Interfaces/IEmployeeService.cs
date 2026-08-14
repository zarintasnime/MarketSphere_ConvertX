using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeListItemDto>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<EmployeeDetailsDto> GetByIDAsync(
        int employeeID,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        CreateEmployeeRequestDto request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        int employeeID,
        UpdateEmployeeRequestDto request,
        CancellationToken cancellationToken = default);
}
