using Microsoft.Extensions.Options;
using Shared.Infrastructure.Storage;

namespace EduConnect.UnitTests;

public sealed class S3FileStorageServiceTests
{
    [Fact]
    public void Construtor_RejeitaConfiguracaoIncompleta()
    {
        var options = Options.Create(new S3Options { Region = "sa-east-1" });

        var exception = Assert.Throws<InvalidOperationException>(() => new S3FileStorageService(options));

        Assert.Equal("A configuração AWS está incompleta.", exception.Message);
    }

    [Fact]
    public async Task UploadAsync_RejeitaArquivoQueNaoEImagemAntesDeAcessarAws()
    {
        var options = Options.Create(new S3Options
        {
            Region = "sa-east-1",
            BucketName = "bucket-unitario",
            AccessKey = "ACCESSKEYUNITTEST",
            SecretKey = "secret-key-used-only-by-unit-tests"
        });
        using var service = new S3FileStorageService(options);
        await using var content = new MemoryStream([1, 2, 3]);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UploadAsync(content, "application/pdf", "arquivo.pdf"));

        Assert.Equal("contentType", exception.ParamName);
    }
}
