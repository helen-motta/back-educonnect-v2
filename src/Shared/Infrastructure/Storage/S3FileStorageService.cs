using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Shared.Domain.Interfaces;

namespace Shared.Infrastructure.Storage;

public sealed class S3FileStorageService : IFileStorageService, IDisposable
{
    private static readonly Dictionary<string, string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
        ["image/gif"] = ".gif"
    };

    private readonly IAmazonS3 _client;
    private readonly S3Options _options;

    public S3FileStorageService(IOptions<S3Options> options)
    {
        _options = options.Value;
        if (string.IsNullOrWhiteSpace(_options.BucketName) ||
            string.IsNullOrWhiteSpace(_options.AccessKey) ||
            string.IsNullOrWhiteSpace(_options.SecretKey))
            throw new InvalidOperationException("A configuração AWS está incompleta.");

        var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
        _client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(_options.Region));
    }

    public async Task<string> UploadAsync(Stream content, string contentType, string originalFileName, CancellationToken cancellationToken = default)
    {
        if (!AllowedContentTypes.TryGetValue(contentType, out var extension))
            throw new ArgumentException("Formato de imagem não permitido.", nameof(contentType));

        var objectKey = $"usuarios/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{extension}";
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            InputStream = content,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await _client.PutObjectAsync(request, cancellationToken);
        var baseUrl = string.IsNullOrWhiteSpace(_options.PublicBaseUrl)
            ? $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com"
            : _options.PublicBaseUrl.TrimEnd('/');
        return $"{baseUrl}/{objectKey}";
    }

    public void Dispose() => _client.Dispose();
}
