using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Domain.Entities.Infrastructure;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.Infrastructure.Services;

public sealed class FileAttachmentService : IFileAttachmentService
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _storage;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public FileAttachmentService(
        IApplicationDbContext db,
        IFileStorageService storage,
        ICurrentUserService currentUser,
        IDateTimeProvider clock)
    {
        _db = db;
        _storage = storage;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<IReadOnlyCollection<FileAttachmentDto>> GetAsync(
        string referenceType,
        int referenceID,
        CancellationToken cancellationToken = default)
    {
        var type = InfrastructureServiceHelper.Required(
            referenceType,
            "Reference type",
            100).ToUpperInvariant();

        var entities = await _db.FileAttachments
            .AsNoTracking()
            .Where(x => x.ReferenceType == type &&
                        x.ReferenceID == referenceID)
            .OrderByDescending(x => x.UploadedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(Map).ToArray();
    }

    public async Task<FileAttachmentDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await InfrastructureServiceHelper.RequireAsync(
            _db.FileAttachments
                .AsNoTracking()
                .Where(x => x.FileAttachmentID == id),
            "File attachment",
            cancellationToken);

        return Map(entity);
    }

    public async Task<int> UploadAsync(
        SaveFileAttachmentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.UserID
            ?? throw new ForbiddenBusinessActionException(
                "Authenticated user is required.");

        if (request.ReferenceID <= 0)
            throw new BusinessRuleException(
                "Reference ID must be greater than zero.");

        var referenceType = InfrastructureServiceHelper.Required(
            request.ReferenceType,
            "Reference type",
            100).ToUpperInvariant();

        var category = InfrastructureServiceHelper.Required(
            request.AttachmentCategory,
            "Attachment category",
            100).ToUpperInvariant();

        var stored = await _storage.SaveAsync(
            request.Content,
            request.OriginalFileName,
            request.MimeType,
            category,
            cancellationToken);

        var entity = new FileAttachment
        {
            ReferenceType = referenceType,
            ReferenceID = request.ReferenceID,
            AttachmentCategory = category,
            FileName = stored.FileName,
            StoredFileName = stored.StoredFileName,
            FileUrl = string.Empty,
            MimeType = stored.MimeType,
            FileSizeBytes = stored.FileSizeBytes,
            FileHash = stored.FileHash,
            IsEvidence = request.IsEvidence,
            CapturedAt = request.CapturedAt,
            GPS = request.GPS?.Trim(),
            VerificationStatus = FileVerificationStatus.Pending,
            UploadedByUserID = userID,
            UploadedAt = _clock.UtcNow
        };

        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        entity.FileUrl = BuildDownloadUrl(entity.FileAttachmentID);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.FileAttachmentID;
    }

    public async Task VerifyAsync(
        int id,
        VerifyFileAttachmentRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.UserID
            ?? throw new ForbiddenBusinessActionException(
                "Authenticated user is required.");

        if (request.VerificationStatus == FileVerificationStatus.Pending)
            throw new BusinessRuleException(
                "Verification status must be Verified or Rejected.");

        var entity = await InfrastructureServiceHelper.RequireAsync(
            _db.FileAttachments.Where(x => x.FileAttachmentID == id),
            "File attachment",
            cancellationToken);

        entity.VerificationStatus = request.VerificationStatus;
        entity.VerifiedByUserID = userID;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Stream> OpenReadAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await InfrastructureServiceHelper.RequireAsync(
            _db.FileAttachments
                .AsNoTracking()
                .Where(x => x.FileAttachmentID == id),
            "File attachment",
            cancellationToken);

        return await _storage.OpenReadAsync(
            entity.StoredFileName,
            cancellationToken);
    }

    private static string BuildDownloadUrl(int id)
        => $"/api/files/{id}/download";

    private static FileAttachmentDto Map(FileAttachment entity)
        => new(
            entity.FileAttachmentID,
            entity.ReferenceType,
            entity.ReferenceID,
            entity.AttachmentCategory,
            entity.FileName,
            entity.StoredFileName,
            BuildDownloadUrl(entity.FileAttachmentID),
            entity.MimeType,
            entity.FileSizeBytes,
            entity.FileHash,
            entity.IsEvidence,
            entity.CapturedAt,
            entity.GPS,
            entity.VerificationStatus,
            entity.VerifiedByUserID,
            entity.UploadedByUserID,
            entity.UploadedAt);
}
