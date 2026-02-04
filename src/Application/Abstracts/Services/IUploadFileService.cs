using Domain;
using Microsoft.AspNetCore.Http;

namespace Application.Abstracts.Services;

public interface IUploadFileService
{
    Task<UploadFile> UploadAsync(IFormFile file, string? displayName, CancellationToken ct);
    Task<List<UploadFile>> UploadMultipleAsync(IFormFileCollection files, CancellationToken ct);

    Task<UploadFile?> GetAsync(int id, CancellationToken ct);
    Task<List<UploadFile>> GetAllAsync(CancellationToken ct);
}
