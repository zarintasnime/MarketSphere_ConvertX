using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Enums;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class FileAttachment
{
    public int FileAttachmentID { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public int ReferenceID { get; set; }
    public string AttachmentCategory { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileHash { get; set; } = string.Empty;
    public bool IsEvidence { get; set; }
    public DateTime? CapturedAt { get; set; }
    public string? GPS { get; set; }
    public FileVerificationStatus VerificationStatus { get; set; } = FileVerificationStatus.Pending;
    public int? VerifiedByUserID { get; set; }
    public int UploadedByUserID { get; set; }
    public DateTime UploadedAt { get; set; }

    public User? VerifiedByUser { get; set; }
    public User UploadedByUser { get; set; } = null!;
}
