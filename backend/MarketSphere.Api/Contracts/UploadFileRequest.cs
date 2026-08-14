using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MarketSphere.Api.Contracts;

public sealed class UploadFileRequest
{
    [Required]
    public IFormFile File { get; init; } = null!;

    [Required]
    public string ReferenceType { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ReferenceID { get; init; }

    [Required]
    public string AttachmentCategory { get; init; } = string.Empty;

    public bool IsEvidence { get; init; }

    public DateTime? CapturedAt { get; init; }

    public string? GPS { get; init; }
}
