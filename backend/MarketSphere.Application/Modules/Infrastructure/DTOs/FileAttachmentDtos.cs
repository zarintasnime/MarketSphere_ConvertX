using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Infrastructure.DTOs;

public sealed record FileAttachmentDto(int FileAttachmentID, string ReferenceType, int ReferenceID, string AttachmentCategory, string FileName, string StoredFileName, string FileUrl, string MimeType, long FileSizeBytes, string FileHash, bool IsEvidence, DateTime? CapturedAt, string? GPS, FileVerificationStatus VerificationStatus, int? VerifiedByUserID, int UploadedByUserID, DateTime UploadedAt);
public sealed class SaveFileAttachmentRequestDto { public string ReferenceType { get; init; } = string.Empty; public int ReferenceID { get; init; } public string AttachmentCategory { get; init; } = string.Empty; public string OriginalFileName { get; init; } = string.Empty; public string MimeType { get; init; } = string.Empty; public Stream Content { get; init; } = Stream.Null; public bool IsEvidence { get; init; } public DateTime? CapturedAt { get; init; } public string? GPS { get; init; } }
public sealed class VerifyFileAttachmentRequestDto { public FileVerificationStatus VerificationStatus { get; init; } public string? Note { get; init; } }
