namespace Shared.Domain.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream content, string contentType, string originalFileName, CancellationToken cancellationToken = default);
}
