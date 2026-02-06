using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;
using Domain.Entities;
using FluentValidation;

namespace Persistence.Services;

public class MediaPropertyService : IMediaPropertyService
{
    private readonly IMediaPropertyRepository _mediaRepo;
    private readonly IPropertyListeningRepository _listingRepo;
    private readonly IFileStorageService _storage;
    private readonly IValidator<CreateMediaProperty> _validator;

    public MediaPropertyService(
        IMediaPropertyRepository mediaRepo,
        IPropertyListeningRepository listingRepo,
        IFileStorageService storage,
        IValidator<CreateMediaProperty> validator)
    {
        _mediaRepo = mediaRepo;
        _listingRepo = listingRepo;
        _storage = storage;
        _validator = validator;
    }

    public async Task<BaseResponse> CreateMediaAsync(CreateMediaProperty dto, CancellationToken ct = default)
    {
        await _validator.ValidateAndThrowAsync(dto, cancellationToken: ct);

        var exists = await _listingRepo.ExistsByIdAsync(dto.PropertyListingId, ct);
        if (!exists) return BaseResponse.Fail("Property tapılmadı.");

        var existingCount = await _mediaRepo.CountByPropertyListingIdAsync(dto.PropertyListingId, ct);
        if (existingCount >= 10) return BaseResponse.Fail("1 elan üçün 10 fotodan çox əlavə etmək olmaz.");
        if (existingCount + dto.Files.Count > 10)
            return BaseResponse.Fail($"Maksimum 10 şəkil olar. Hazırda var: {existingCount}, göndərmisən: {dto.Files.Count}.");

        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png" };

        var maxOrder = await _mediaRepo.GetMaxOrderByPropertyListingIdAsync(dto.PropertyListingId, ct);
        var order = maxOrder;

        foreach (var file in dto.Files)
        {
            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !allowed.Contains(ext))
                return BaseResponse.Fail("Yalnız .jpg, .jpeg, .png qəbul olunur.");

            var objectKey = await _storage.SaveAsync(
                file.OpenReadStream(),
                file.FileName,          // ext buradan götürülür
                file.ContentType,
                dto.PropertyListingId,
                ct);

            var media = new MediaProperty
            {
                PropertyListingId = dto.PropertyListingId,
                ObjectKey = objectKey,
                MediaType = "image",
                Order = ++order
            };

            await _mediaRepo.AddAsync(media, ct);
        }

        await _mediaRepo.SaveChangesAsync(ct);

        return BaseResponse.Ok($"Şəkillər əlavə olundu. Yeni toplam: {existingCount + dto.Files.Count}");
    }


    // Create-only dedin deyə qalanları hələlik belə qala bilər
    public Task<BaseResponse> DeleteByIdMediaAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<BaseResponse<List<GetAllMediaProperty>>> GetAllMediaAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<BaseResponse<GetByIdMediaProperty?>> GetByIdMediaAsync(int id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<BaseResponse> UpdatePropertyAsync(int id, CreateMediaProperty dto, CancellationToken ct = default)
        => throw new NotImplementedException();
}
