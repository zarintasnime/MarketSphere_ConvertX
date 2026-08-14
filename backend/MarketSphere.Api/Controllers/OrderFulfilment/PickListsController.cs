using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.OrderFulfilment;

[ApiController]
[Authorize]
[Route("api/pick-lists")]
public sealed class PickListsController : ControllerBase
{
    private readonly IPickListService _service;

    public PickListsController(IPickListService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.PickListsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<PickListListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<PickListListDto>>.Success(result, "Pick lists retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.PickListsView)]
    public async Task<ActionResult<ApiResponse<PickListDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<PickListDetailsDto>.Success(result, "Pick list retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.PickListsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreatePickListRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Pick list created successfully."));
    }

    [HttpPost("{id:int}/release")]
    [HasPermission(PermissionCodes.PickListsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Release(
        int id,
        [FromBody] ReleasePickListRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ReleaseAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Pick list released successfully."));
    }

    [HttpPost("{id:int}/record-pick")]
    [HasPermission(PermissionCodes.PickListsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> RecordPick(
        int id,
        [FromBody] RecordPickRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.RecordPickAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Pick quantities recorded successfully."));
    }

    [HttpPost("{id:int}/verify")]
    [HasPermission(PermissionCodes.PickListsVerify)]
    public async Task<ActionResult<ApiResponse<bool>>> Verify(
        int id,
        [FromBody] VerifyPickListRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.VerifyAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Pick list verified successfully."));
    }
}
