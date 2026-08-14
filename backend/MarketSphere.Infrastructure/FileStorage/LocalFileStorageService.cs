using MarketSphere.Application.Common.DTOs;
using MarketSphere.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;

namespace MarketSphere.Infrastructure.FileStorage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadRoot;

    public LocalFileStorageService(IHostEnvironment environment)
    {
        _uploadRoot = Path.Combine(
            environment.ContentRootPath,
            "App_Data",
            "Uploads");
    }

    public async Task<FileUploadResultDto> SaveAsync(
        Stream stream,
        string originalFileName,
        string mimeType,
        string category,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
            throw new ArgumentException(
                "The file stream must be readable.",
                nameof(stream));

        var safeFileName = FilePathBuilder.GetSafeOriginalFileName(
            originalFileName);
        var safeCategory = FilePathBuilder.NormalizeCategory(category);
        var extension = Path.GetExtension(safeFileName);
        var generatedName = $"{Guid.NewGuid():N}{extension}";
        var relativePath = $"{safeCategory}/{generatedName}";
        var normalizedRelativePath = relativePath.Replace('\\', '/');
        var fullPath = FilePathBuilder.ResolveUnderRoot(
            _uploadRoot,
            normalizedRelativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var output = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        using var hashAlgorithm = IncrementalHash.CreateHash(
            HashAlgorithmName.SHA256);

        var buffer = new byte[81920];
        long totalBytes = 0;

        while (true)
        {
            var bytesRead = await stream.ReadAsync(
                buffer.AsMemory(),
                cancellationToken);

            if (bytesRead == 0)
                break;

            await output.WriteAsync(
                buffer.AsMemory(0, bytesRead),
                cancellationToken);

            hashAlgorithm.AppendData(buffer, 0, bytesRead);
            totalBytes += bytesRead;
        }

        if (totalBytes == 0)
        {
            await output.DisposeAsync();
            File.Delete(fullPath);
            throw new InvalidOperationException(
                "An empty file cannot be stored.");
        }

        var fileHash = Convert.ToHexString(
            hashAlgorithm.GetHashAndReset());

        return new FileUploadResultDto(
            safeFileName,
            normalizedRelativePath,
            string.Empty,
            string.IsNullOrWhiteSpace(mimeType)
                ? "application/octet-stream"
                : mimeType.Trim(),
            totalBytes,
            fileHash);
    }

    public Task<Stream> OpenReadAsync(
        string storedFileName,
        CancellationToken cancellationToken = default)
    {
        var fullPath = FilePathBuilder.ResolveUnderRoot(
            _uploadRoot,
            storedFileName);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException(
                "The stored file was not found.",
                storedFileName);

        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        return Task.FromResult(stream);
    }
}
