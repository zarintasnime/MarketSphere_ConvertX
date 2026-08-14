using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;
using MarketSphere.Application.Modules.MarketingField.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.MarketingField;

[ApiController]
[Authorize]
[Route("api/visits")]
public sealed class VisitsController : ControllerBase
{
    private readonly IVisitService _service;

    public VisitsController(IVisitService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.VisitsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<VisitListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<VisitListDto>>.Success(result, "Visits retrieved successfully."));
    }

    [HttpGet("{visitID:int}")]
    [HasPermission(PermissionCodes.VisitsView)]
    public async Task<ActionResult<ApiResponse<VisitDetailsDto>>> GetById(
            int visitID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(visitID, cancellationToken);
        return Ok(ApiResponse<VisitDetailsDto>.Success(result, "Visit retrieved successfully."));
    }

    [HttpPost("check-in")]
    [HasPermission(PermissionCodes.VisitsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CheckIn(
        [FromBody] CheckInVisitRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CheckInAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Visit check-in completed successfully."));
    }

    [HttpPost("{visitID:int}/check-out")]
    [HasPermission(PermissionCodes.VisitsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> CheckOut(
        int visitID,
        [FromBody] CheckOutVisitRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.CheckOutAsync(visitID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Visit check-out completed successfully."));
    }

    [HttpPost("{visitID:int}/cancel")]
    [HasPermission(PermissionCodes.VisitsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Cancel(
        int visitID,
        [FromBody] CancelVisitRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.CancelAsync(visitID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Visit cancelled successfully."));
    }
}
