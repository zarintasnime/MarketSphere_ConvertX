using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.KPI.DTOs;
using MarketSphere.Application.Modules.KPI.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.KPI;

[ApiController]
[Authorize]
[Route("api/rewards")]
public sealed class RewardsController : ControllerBase
{
    private readonly IRewardService _service;

    public RewardsController(IRewardService service) => _service = service;

    [HttpGet("rules")]
    [HasPermission(PermissionCodes.RewardsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<RewardRuleDto>>>> GetRules(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetRulesAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<RewardRuleDto>>.Success(result, "Reward rules retrieved successfully."));
    }

    [HttpPost("rules")]
    [HasPermission(PermissionCodes.RewardsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateRule(
        [FromBody] SaveRewardRuleRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateRuleAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Reward rule created successfully."));
    }

    [HttpPut("rules/{id:int}")]
    [HasPermission(PermissionCodes.RewardsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateRule(
        int id,
        [FromBody] SaveRewardRuleRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateRuleAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Reward rule updated successfully."));
    }

    [HttpPost("calculate")]
    [HasPermission(PermissionCodes.RewardsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Calculate(
        [FromBody] CalculateRewardRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CalculateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Reward calculation completed successfully."));
    }

    [HttpGet("calculations")]
    [HasPermission(PermissionCodes.RewardsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<RewardCalculationDto>>>> GetCalculations(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetCalculationsAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<RewardCalculationDto>>.Success(result, "Reward calculations retrieved successfully."));
    }

    [HttpPost("calculations/{id:int}/adjust")]
    [HasPermission(PermissionCodes.RewardsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Adjust(
        int id,
        [FromBody] AdjustRewardRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.AdjustAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Reward calculation adjusted successfully."));
    }

    [HttpPatch("calculations/{id:int}/status")]
    [HasPermission(PermissionCodes.RewardsApprove)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int id,
        [FromBody] ChangeRewardStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Reward calculation status changed successfully."));
    }
}
