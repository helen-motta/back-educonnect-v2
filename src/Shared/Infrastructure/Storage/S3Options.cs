namespace Shared.Infrastructure.Storage;

public sealed class S3Options
{
    public const string SectionName = "AWS";
    public string Region { get; init; } = "sa-east-1";
    public string BucketName { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string? PublicBaseUrl { get; init; }
}
