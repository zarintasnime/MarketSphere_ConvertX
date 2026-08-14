using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.CRM;

[ApiController]
[Authorize]
[Route("api/quotations")]
public sealed class QuotationsController : ControllerBase
{
    private readonly IQuotationService _service;

    public QuotationsController(IQuotationService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.QuotationsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<QuotationListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<QuotationListDto>>.Success(result, "Quotations retrieved successfully."));
    }

    [HttpGet("{quotationID:int}")]
    [HasPermission(PermissionCodes.QuotationsView)]
    public async Task<ActionResult<ApiResponse<QuotationDetailsDto>>> GetById(
            int quotationID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(quotationID, cancellationToken);
        return Ok(ApiResponse<QuotationDetailsDto>.Success(result, "Quotation retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.QuotationsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateDraft(
        [FromBody] SaveQuotationRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateDraftAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Quotation draft created successfully."));
    }

    [HttpPut("{quotationID:int}")]
    [HasPermission(PermissionCodes.QuotationsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateDraft(
        int quotationID,
        [FromBody] SaveQuotationRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateDraftAsync(quotationID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Quotation draft updated successfully."));
    }

    [HttpPost("{quotationID:int}/versions")]
    [HasPermission(PermissionCodes.QuotationsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateNewVersion(
        int quotationID,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateNewVersionAsync(quotationID, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Quotation version created successfully."));
    }

    [HttpPatch("{quotationID:int}/status")]
    [HasPermission(PermissionCodes.QuotationsApprove)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int quotationID,
        [FromBody] ChangeQuotationStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(quotationID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Quotation status changed successfully."));
    }
}
