using MarketSphere.Api.Authorization;
using MarketSphere.Api.Contracts;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.Infrastructure;

[ApiController]
[Authorize]
[Route("api/files")]
public sealed class FilesController : ControllerBase
{
    private readonly IFileAttachmentService _service;

    public FilesController(IFileAttachmentService service)
    {
        _service = service;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.FilesView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<FileAttachmentDto>>>> Get(
        [FromQuery] string referenceType,
        [FromQuery] int referenceID,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(
            referenceType,
            referenceID,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<FileAttachmentDto>>.Success(
            result,
            "File attachments retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.FilesView)]
    public async Task<ActionResult<ApiResponse<FileAttachmentDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);

        return Ok(ApiResponse<FileAttachmentDto>.Success(
            result,
            "File attachment retrieved successfully."));
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(100_000_000)]
    [HasPermission(PermissionCodes.FilesManage)]
    public async Task<ActionResult<ApiResponse<int>>> Upload(
        [FromForm] UploadFileRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            throw new BusinessRuleException("A non-empty file is required.");
        }

        await using var stream = request.File.OpenReadStream();

        var saveRequest = new SaveFileAttachmentRequestDto
        {
            ReferenceType = request.ReferenceType,
            ReferenceID = request.ReferenceID,
            AttachmentCategory = request.AttachmentCategory,
            OriginalFileName = request.File.FileName,
            MimeType = string.IsNullOrWhiteSpace(request.File.ContentType)
                ? "application/octet-stream"
                : request.File.ContentType,
            Content = stream,
            IsEvidence = request.IsEvidence,
            CapturedAt = request.CapturedAt,
            GPS = request.GPS
        };

        var id = await _service.UploadAsync(
            saveRequest,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            ApiResponse<int>.Success(
                id,
                "File uploaded successfully."));
    }

    [HttpPost("{id:int}/verify")]
    [HasPermission(PermissionCodes.FilesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Verify(
        int id,
        [FromBody] VerifyFileAttachmentRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.VerifyAsync(
            id,
            request,
            cancellationToken);

        return Ok(ApiResponse<bool>.Success(
            true,
            "File verification completed successfully."));
    }

    [HttpGet("{id:int}/download")]
    [HasPermission(PermissionCodes.FilesView)]
    public async Task<IActionResult> Download(
        int id,
        CancellationToken cancellationToken)
    {
        var attachment = await _service.GetByIdAsync(
            id,
            cancellationToken);

        var stream = await _service.OpenReadAsync(
            id,
            cancellationToken);

        return File(
            stream,
            attachment.MimeType,
            attachment.FileName,
            enableRangeProcessing: true);
    }
}
