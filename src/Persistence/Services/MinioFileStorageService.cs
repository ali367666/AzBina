using Application.Abstracts.Services;

namespace Persistence.Services;

public class MinioFileStorageService : IFileStorageService
{
    public Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        int propertyAdId,
        CancellationToken ct = default)
    {
        // Hələlik fake implementation
        // Sonra MinIO SDK ilə dolduracağıq
        return Task.FromResult(fileName);
    }

    public Task DeleteFileAsync(string objectKey, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}
