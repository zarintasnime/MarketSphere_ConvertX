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
[Route("api/market-observations")]
public sealed class MarketObservationsController : ControllerBase
{
    private readonly IMarketObservationService _service;

    public MarketObservationsController(IMarketObservationService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.MarketObservationsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<MarketObservationListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<MarketObservationListDto>>.Success(result, "Market observations retrieved successfully."));
    }

    [HttpGet("{marketObservationID:int}")]
    [HasPermission(PermissionCodes.MarketObservationsView)]
    public async Task<ActionResult<ApiResponse<MarketObservationDetailsDto>>> GetById(
            int marketObservationID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(marketObservationID, cancellationToken);
        return Ok(ApiResponse<MarketObservationDetailsDto>.Success(result, "Market observation retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.MarketObservationsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveMarketObservationRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Market observation created successfully."));
    }

    [HttpPut("{marketObservationID:int}")]
    [HasPermission(PermissionCodes.MarketObservationsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int marketObservationID,
        [FromBody] SaveMarketObservationRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(marketObservationID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Market observation updated successfully."));
    }

    [HttpDelete("{marketObservationID:int}")]
    [HasPermission(PermissionCodes.MarketObservationsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        int marketObservationID,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(marketObservationID, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Market observation deleted successfully."));
    }
}
