using MarketSphere.Application.Common.DTOs;

namespace MarketSphere.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<FileUploadResultDto> SaveAsync(
        Stream stream,
        string originalFileName,
        string mimeType,
        string category,
        CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(
        string storedFileName,
        CancellationToken cancellationToken = default);
}
