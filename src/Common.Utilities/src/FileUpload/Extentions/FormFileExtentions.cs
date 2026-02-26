using Microsoft.AspNetCore.Http;

namespace Common.Utilities.FileUpload;

/// <summary>
/// Provides robust image upload functionality with validation and secure path handling.
/// </summary>
public static partial class FormFileExtentions
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    ];

    /// <summary>
    /// Uploads an image to the specified folder and returns the absolute path as Uri.
    /// </summary>
    /// <param name="image">The image file to upload.</param>
    /// <param name="destination">The target folder path (can be absolute or relative to current directory).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Absolute Uri to the uploaded image.</returns>
    /// <exception cref="NotAnImageException">Thrown when the file is not a valid image.</exception>
    /// <exception cref="UploadFailedException">Thrown when the upload fails due to invalid paths or unexpected errors.</exception>
    public static async Task<string> UploadAsImageAsync(
        this IFormFile image,
        string destination,
        CancellationToken cancellationToken = default)
    {
        AssertContentType(image.ContentType);

        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        AssertExtension(extension);

        var uploadPath = Path.IsPathRooted(destination)
            ? destination
            : Path.Combine(Directory.GetCurrentDirectory(), destination);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadPath, fileName);

        try { Directory.CreateDirectory(uploadPath); }
        catch
        {
            throw new UploadFailedException($"Cannot create upload directory: {uploadPath}");
        }

        try
        {
            await using var stream = new FileStream(
                                         filePath,
                                         FileMode.Create,
                                         FileAccess.Write,
                                         FileShare.None,
                                         bufferSize: 81920,
                                         useAsync: true);

            await image.CopyToAsync(stream, cancellationToken);
        }
        catch (IOException ex)
        {
            throw new UploadFailedException($"Failed to write image to {filePath}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UploadFailedException($"Access denied to path {filePath}", ex);
        }
        catch (Exception ex)
        {
            throw new UploadFailedException($"Unexpected error during upload: {ex.Message}", ex);
        }

        return $"{destination}/{fileName}";
    }

    private static void AssertContentType(string contentType)
    {
        if (!AllowedContentTypes.Contains(contentType.ToLowerInvariant()))
        {
            throw new NotAnImageException(contentType);
        }
    }

    private static void AssertExtension(string extension)
    {
        if (!AllowedExtensions.Contains(extension))
        {
            throw new NotAnImageException($"Unknown extension: {extension}");
        }
    }

}
