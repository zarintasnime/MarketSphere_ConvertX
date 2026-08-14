namespace MarketSphere.Infrastructure.FileStorage;

public static class FilePathBuilder
{
    public static string NormalizeCategory(string category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        var trimmed = category.Trim();

        if (trimmed is "." or ".." ||
            trimmed.IndexOfAny(
                Path.GetInvalidFileNameChars()) >= 0 ||
            trimmed.Contains(
                Path.DirectorySeparatorChar) ||
            trimmed.Contains(
                Path.AltDirectorySeparatorChar))
        {
            throw new ArgumentException(
                "The storage category is invalid.",
                nameof(category));
        }

        return trimmed;
    }

    public static string GetSafeOriginalFileName(
        string originalFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            originalFileName);

        var safeName = Path.GetFileName(
            originalFileName.Trim());

        if (string.IsNullOrWhiteSpace(safeName) ||
            safeName is "." or "..")
        {
            throw new ArgumentException(
                "The original file name is invalid.",
                nameof(originalFileName));
        }

        return safeName;
    }

    public static string ResolveUnderRoot(
        string root,
        string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        var normalizedRoot = Path.GetFullPath(root)
            .TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        var fullPath = Path.GetFullPath(
            Path.Combine(
                normalizedRoot,
                relativePath.Replace(
                    '/',
                    Path.DirectorySeparatorChar)));

        if (!fullPath.StartsWith(
                normalizedRoot,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The resolved file path is outside the upload root.");
        }

        return fullPath;
    }
}
