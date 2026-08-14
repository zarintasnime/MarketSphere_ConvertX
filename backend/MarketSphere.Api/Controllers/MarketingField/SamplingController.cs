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
[Route("api/sampling")]
public sealed class SamplingController : ControllerBase
{
    private readonly ISamplingService _service;

    public SamplingController(ISamplingService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.SamplingView)]
    public async Task<ActionResult<ApiResponse<PagedResult<SamplingLogListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<SamplingLogListDto>>.Success(result, "Sampling logs retrieved successfully."));
    }

    [HttpGet("{samplingLogID:int}")]
    [HasPermission(PermissionCodes.SamplingView)]
    public async Task<ActionResult<ApiResponse<SamplingLogDetailsDto>>> GetById(
            int samplingLogID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(samplingLogID, cancellationToken);
        return Ok(ApiResponse<SamplingLogDetailsDto>.Success(result, "Sampling log retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.SamplingManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveSamplingLogRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Sampling log created successfully."));
    }

    [HttpPut("{samplingLogID:int}")]
    [HasPermission(PermissionCodes.SamplingManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int samplingLogID,
        [FromBody] SaveSamplingLogRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(samplingLogID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Sampling log updated successfully."));
    }

    [HttpDelete("{samplingLogID:int}")]
    [HasPermission(PermissionCodes.SamplingManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        int samplingLogID,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(samplingLogID, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Sampling log deleted successfully."));
    }
}
