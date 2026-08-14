using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Services;

public sealed class OrganizationService : IOrganizationService
{
    private readonly IApplicationDbContext _db;

    public OrganizationService(IApplicationDbContext db)
    {
        _db = db;
    }

    public Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CompanyDto> items = _db.Companies
            .OrderBy(x => x.CompanyName)
            .Select(x => new CompanyDto(
                x.CompanyID,
                x.CompanyCode,
                x.CompanyName,
                x.TradeLicenseNo,
                x.Phone,
                x.Email,
                x.Address,
                x.IsActive))
            .ToArray();

        return Task.FromResult(items);
    }

    public async Task UpdateCompanyAsync(
        int companyID,
        UpdateCompanyRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var company = _db.Companies.FirstOrDefault(
            x => x.CompanyID == companyID)
            ?? throw new NotFoundException(
                $"Company with ID {companyID} was not found.");

        ValidationHelper.RequireNotBlank(
            request.CompanyName,
            nameof(request.CompanyName),
            150);

        company.CompanyName = request.CompanyName.Trim();
        company.TradeLicenseNo =
            request.TradeLicenseNo.NullIfWhiteSpace();
        company.Phone = request.Phone.NullIfWhiteSpace();
        company.Email =
            request.Email.NullIfWhiteSpace()?.NormalizeEmail();
        company.Address =
            request.Address.NullIfWhiteSpace();
        company.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<BranchDto>> GetBranchesAsync(
        int? companyID = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Branches;

        if (companyID.HasValue)
        {
            query = query.Where(
                x => x.CompanyID == companyID.Value);
        }

        IReadOnlyCollection<BranchDto> items = query
            .OrderBy(x => x.BranchName)
            .Select(x => new BranchDto(
                x.BranchID,
                x.CompanyID,
                x.BranchCode,
                x.BranchName,
                x.BranchType,
                x.Address,
                x.Phone,
                x.IsActive))
            .ToArray();

        return Task.FromResult(items);
    }

    public async Task<int> CreateBranchAsync(
        CreateBranchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!_db.Companies.Any(
                x => x.CompanyID == request.CompanyID &&
                     x.IsActive))
        {
            throw new NotFoundException(
                $"Company with ID {request.CompanyID} was not found or is inactive.");
        }

        ValidationHelper.RequireNotBlank(
            request.BranchCode,
            nameof(request.BranchCode),
            50);

        ValidationHelper.RequireNotBlank(
            request.BranchName,
            nameof(request.BranchName),
            150);

        var branchCode = request.BranchCode.NormalizeCode();

        if (_db.Branches.Any(
                x => x.CompanyID == request.CompanyID &&
                     x.BranchCode == branchCode))
        {
            throw new ConflictException(
                $"Branch code '{branchCode}' already exists for this company.");
        }

        var branch = new Branch
        {
            CompanyID = request.CompanyID,
            BranchCode = branchCode,
            BranchName = request.BranchName.Trim(),
            BranchType = request.BranchType,
            Address = request.Address.NullIfWhiteSpace(),
            Phone = request.Phone.NullIfWhiteSpace(),
            IsActive = true
        };

        await _db.AddAsync(branch, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return branch.BranchID;
    }

    public async Task UpdateBranchAsync(
        int branchID,
        UpdateBranchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var branch = _db.Branches.FirstOrDefault(
            x => x.BranchID == branchID)
            ?? throw new NotFoundException(
                $"Branch with ID {branchID} was not found.");

        ValidationHelper.RequireNotBlank(
            request.BranchName,
            nameof(request.BranchName),
            150);

        branch.BranchName = request.BranchName.Trim();
        branch.BranchType = request.BranchType;
        branch.Address = request.Address.NullIfWhiteSpace();
        branch.Phone = request.Phone.NullIfWhiteSpace();
        branch.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
