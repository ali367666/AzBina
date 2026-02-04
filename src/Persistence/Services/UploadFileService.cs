using Application.Abstracts.Services;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Services;

public class UploadFileService : IUploadFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly BinaDbContext _db;

    private const long MaxBytes = 10 * 1024 * 1024; // 10MB

    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png" };

    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png" };

    public UploadFileService(IWebHostEnvironment env, BinaDbContext db)
    {
        _env = env;
        _db = db;
    }

    public async Task<UploadFile> UploadAsync(IFormFile file, string? displayName, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded.");

        if (file.Length > MaxBytes)
            throw new ArgumentException("File size must be <= 10MB.");

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            throw new ArgumentException("Only image files are allowed (.jpg, .jpeg, .png).");

        if (!AllowedContentTypes.Contains(file.ContentType))
            throw new ArgumentException("Invalid content type.");

        // wwwroot fallback
        var webRoot = _env.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
            webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");

        var folderAbs = Path.Combine(webRoot, "uploads", "images");
        Directory.CreateDirectory(folderAbs);

        var newName = $"{Guid.NewGuid():N}{ext}";
        var absPath = Path.Combine(folderAbs, newName);

        await using (var stream = new FileStream(absPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await file.CopyToAsync(stream, ct);
        }

        var url = $"/uploads/images/{newName}";
        var originalName = Path.GetFileName(file.FileName);

        var entity = new UploadFile
        {
            FileName = string.IsNullOrWhiteSpace(displayName) ? originalName : displayName.Trim(),
            FileUrl = url
        };

        _db.Set<UploadFile>().Add(entity);
        await _db.SaveChangesAsync(ct);

        return entity;
    }

    public async Task<List<UploadFile>> UploadMultipleAsync(IFormFileCollection files, CancellationToken ct)
    {
        var list = new List<UploadFile>();

        foreach (var file in files)
        {
            var uploaded = await UploadAsync(file, null, ct);
            list.Add(uploaded);
        }

        return list;
    }

    public async Task<UploadFile?> GetAsync(int id, CancellationToken ct)
    {
        return await _db.Set<UploadFile>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<UploadFile>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Set<UploadFile>()
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync(ct);
    }
}
