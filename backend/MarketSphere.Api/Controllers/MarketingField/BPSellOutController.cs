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
[Route("api/bp-sell-out")]
public sealed class BPSellOutController : ControllerBase
{
    private readonly IBPSellOutService _service;

    public BPSellOutController(IBPSellOutService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.BpSellOutView)]
    public async Task<ActionResult<ApiResponse<PagedResult<BPSellOutListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<BPSellOutListDto>>.Success(result, "BP sell-out records retrieved successfully."));
    }

    [HttpGet("{bpSellOutID:int}")]
    [HasPermission(PermissionCodes.BpSellOutView)]
    public async Task<ActionResult<ApiResponse<BPSellOutDetailsDto>>> GetById(
            int bpSellOutID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(bpSellOutID, cancellationToken);
        return Ok(ApiResponse<BPSellOutDetailsDto>.Success(result, "BP sell-out record retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.BpSellOutManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveBPSellOutRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "BP sell-out record created successfully."));
    }

    [HttpPut("{bpSellOutID:int}")]
    [HasPermission(PermissionCodes.BpSellOutManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int bpSellOutID,
        [FromBody] SaveBPSellOutRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(bpSellOutID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "BP sell-out record updated successfully."));
    }

    [HttpPost("{bpSellOutID:int}/verify")]
    [HasPermission(PermissionCodes.BpSellOutVerify)]
    public async Task<ActionResult<ApiResponse<bool>>> Verify(
        int bpSellOutID,
        [FromBody] VerifyBPSellOutRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.VerifyAsync(bpSellOutID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "BP sell-out record verified successfully."));
    }
}
