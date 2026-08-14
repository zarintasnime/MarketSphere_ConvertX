using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.OrganizationSecurity;

[ApiController]
[Authorize]
[Route("api/organization")]
public sealed class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _service;

    public OrganizationController(IOrganizationService service) => _service = service;

    [HttpGet("companies")]
    [HasPermission(PermissionCodes.OrganizationView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CompanyDto>>>> GetCompanies(
            CancellationToken cancellationToken)
    {
        var result = await _service.GetCompaniesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<CompanyDto>>.Success(result, "Companies retrieved successfully."));
    }

    [HttpPut("companies/{companyID:int}")]
    [HasPermission(PermissionCodes.OrganizationManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCompany(
        int companyID,
        [FromBody] UpdateCompanyRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateCompanyAsync(companyID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Company updated successfully."));
    }

    [HttpGet("branches")]
    [HasPermission(PermissionCodes.OrganizationView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<BranchDto>>>> GetBranches(
            [FromQuery] int? companyID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetBranchesAsync(companyID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<BranchDto>>.Success(result, "Branches retrieved successfully."));
    }

    [HttpPost("branches")]
    [HasPermission(PermissionCodes.OrganizationManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateBranch(
        [FromBody] CreateBranchRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateBranchAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Branch created successfully."));
    }

    [HttpPut("branches/{branchID:int}")]
    [HasPermission(PermissionCodes.OrganizationManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateBranch(
        int branchID,
        [FromBody] UpdateBranchRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateBranchAsync(branchID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Branch updated successfully."));
    }
}
