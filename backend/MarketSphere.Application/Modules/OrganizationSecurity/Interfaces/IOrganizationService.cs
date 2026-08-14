using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;

public interface IOrganizationService
{
    Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync(
        CancellationToken cancellationToken = default);

    Task UpdateCompanyAsync(
        int companyID,
        UpdateCompanyRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BranchDto>> GetBranchesAsync(
        int? companyID = null,
        CancellationToken cancellationToken = default);

    Task<int> CreateBranchAsync(
        CreateBranchRequestDto request,
        CancellationToken cancellationToken = default);

    Task UpdateBranchAsync(
        int branchID,
        UpdateBranchRequestDto request,
        CancellationToken cancellationToken = default);
}
