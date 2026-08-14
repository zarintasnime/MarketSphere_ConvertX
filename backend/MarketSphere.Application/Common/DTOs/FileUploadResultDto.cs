namespace MarketSphere.Application.Common.DTOs;

public sealed record FileUploadResultDto(
    string FileName,
    string StoredFileName,
    string FileUrl,
    string MimeType,
    long FileSizeBytes,
    string FileHash);
