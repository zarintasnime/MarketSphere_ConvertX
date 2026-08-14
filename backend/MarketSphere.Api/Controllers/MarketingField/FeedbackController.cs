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
[Route("api/feedback")]
public sealed class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _service;

    public FeedbackController(IFeedbackService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.FeedbackView)]
    public async Task<ActionResult<ApiResponse<PagedResult<FeedbackListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<FeedbackListDto>>.Success(result, "Feedbacks retrieved successfully."));
    }

    [HttpGet("{feedbackID:int}")]
    [HasPermission(PermissionCodes.FeedbackView)]
    public async Task<ActionResult<ApiResponse<FeedbackDetailsDto>>> GetById(
            int feedbackID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(feedbackID, cancellationToken);
        return Ok(ApiResponse<FeedbackDetailsDto>.Success(result, "Feedback retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.FeedbackManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveFeedbackRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Feedback created successfully."));
    }

    [HttpPut("{feedbackID:int}")]
    [HasPermission(PermissionCodes.FeedbackManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int feedbackID,
        [FromBody] SaveFeedbackRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(feedbackID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Feedback updated successfully."));
    }

    [HttpDelete("{feedbackID:int}")]
    [HasPermission(PermissionCodes.FeedbackManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        int feedbackID,
        CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(feedbackID, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Feedback deleted successfully."));
    }
}
