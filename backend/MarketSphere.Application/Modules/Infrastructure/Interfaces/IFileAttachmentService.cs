using MarketSphere.Application.Modules.Infrastructure.DTOs;

namespace MarketSphere.Application.Modules.Infrastructure.Interfaces;

public interface IFileAttachmentService
{
    Task<IReadOnlyCollection<FileAttachmentDto>> GetAsync(string referenceType, int referenceID, CancellationToken cancellationToken = default);
    Task<FileAttachmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> UploadAsync(SaveFileAttachmentRequestDto request, CancellationToken cancellationToken = default);
    Task VerifyAsync(int id, VerifyFileAttachmentRequestDto request, CancellationToken cancellationToken = default);
    Task<Stream> OpenReadAsync(int id, CancellationToken cancellationToken = default);
}
