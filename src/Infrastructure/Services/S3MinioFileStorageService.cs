using Application.Abstracts.Services;
using Application.Options;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.Services;

public class S3MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public S3MinioFileStorageService(
        IMinioClient minioClient,
        IOptions<MinioOptions> options)
    {
        _minioClient = minioClient;
        _options = options.Value;
    }

    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        int propertyAdId,
        CancellationToken ct = default)
    {
        // 1️⃣ Bucket var?
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(_options.Bucket);

        bool exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, ct);

        if (!exists)
        {
            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket(_options.Bucket);

            await _minioClient.MakeBucketAsync(makeBucketArgs, ct);
        }

        // 2️⃣ ObjectKey yarat
        var extension = Path.GetExtension(fileName);
        var objectKey = $"{propertyAdId}/{Guid.NewGuid():N}{extension}";

        // 3️⃣ Stream ölçüsünü tap
        long size;
        if (content.CanSeek)
        {
            size = content.Length;
        }
        else
        {
            var ms = new MemoryStream();
            await content.CopyToAsync(ms, ct);
            ms.Position = 0;
            content = ms;
            size = ms.Length;
        }

        // 4️⃣ Upload
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(size)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs, ct);

        // 5️⃣ DB üçün objectKey qaytar
        return objectKey;
    }

    public async Task DeleteFileAsync(string objectKey, CancellationToken ct = default)
    {
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(removeArgs, ct);
    }
}
