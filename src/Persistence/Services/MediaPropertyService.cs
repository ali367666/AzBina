using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;
using AutoMapper;
using FluentValidation;
using Domain.Entities;

namespace Persistence.Services;

public class MediaPropertyService : IMediaPropertyService
{
    private readonly IMediaPropertyRepository _mediaPropertyRepository;
    private readonly IPropertyListeningRepository _propertyListingRepository;
    private readonly IMapper _mapper;

    // ✅ DTO validator olmalıdır (validator class yox)
    private readonly IValidator<CreateMediaProperty> _createValidator;

    public MediaPropertyService(
        IMediaPropertyRepository mediaPropertyRepository,
        IPropertyListeningRepository propertyListingRepository,
        IMapper mapper,
        IValidator<CreateMediaProperty> createValidator)
    {
        _mediaPropertyRepository = mediaPropertyRepository;
        _propertyListingRepository = propertyListingRepository;
        _mapper = mapper;
        _createValidator = createValidator;
    }


    public async Task<BaseResponse> CreateMediaAsync(
        CreateMediaProperty dto,
        CancellationToken ct = default)
    {
        // 1️⃣ DTO validation
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken: ct);

        // 2️⃣ Property mövcuddurmu?
        var propertyExists =
            await _propertyListingRepository.ExistsByIdAsync(dto.PropertyListingId, ct);

        if (!propertyExists)
            return BaseResponse.Fail("Property tapılmadı.");

        // 3️⃣ 1 elan üçün maksimum 10 foto
        var count =
            await _mediaPropertyRepository.CountByPropertyListingIdAsync(
                dto.PropertyListingId, ct);

        if (count >= 10)
            return BaseResponse.Fail("1 elan üçün 10 fotodan çox əlavə etmək olmaz.");

        // 4️⃣ Order avtomatik (1..10)
        var maxOrder =
            await _mediaPropertyRepository.GetMaxOrderByPropertyListingIdAsync(
                dto.PropertyListingId, ct);

        var nextOrder = maxOrder + 1;

        if (nextOrder > 10)
            return BaseResponse.Fail("1 elan üçün 10 fotodan çox əlavə etmək olmaz.");

        // 5️⃣ Map + save
        var media = _mapper.Map<MediaProperty>(dto);
        media.Order = nextOrder;

        await _mediaPropertyRepository.AddAsync(media, ct);
        await _mediaPropertyRepository.SaveChangesAsync(ct);

        return BaseResponse.Ok($"Media əlavə olundu (Order = {nextOrder}).");
    }




    public async Task<BaseResponse> DeleteByIdMediaAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse.Fail("Id düzgün deyil.");

        var media = await _mediaPropertyRepository.GetByIdAsync(id, ct);
        if (media is null)
            return BaseResponse.Fail($"Id={id} olan Media tapılmadı.");

        _mediaPropertyRepository.Delete(media);
        await _mediaPropertyRepository.SaveChangesAsync(ct);

        return BaseResponse.Ok("Media silindi.");
    }

    public async Task<BaseResponse<List<GetAllMediaProperty>>> GetAllMediaAsync(CancellationToken ct = default)
    {
        var medias = await _mediaPropertyRepository.GetAllAsync(ct);

        var list = medias?.ToList() ?? new List<MediaProperty>();
        var data = _mapper.Map<List<GetAllMediaProperty>>(list);

        return BaseResponse<List<GetAllMediaProperty>>.Ok(data);
    }

    public async Task<BaseResponse<GetByIdMediaProperty?>> GetByIdMediaAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse<GetByIdMediaProperty?>.Fail("Id düzgün deyil.");

        var media = await _mediaPropertyRepository.GetByIdAsync(id, ct);
        if (media is null)
            return BaseResponse<GetByIdMediaProperty?>.Fail($"Id={id} olan Media tapılmadı.");

        var data = _mapper.Map<GetByIdMediaProperty>(media);
        return BaseResponse<GetByIdMediaProperty?>.Ok(data, "Media tapıldı.");
    }

    public async Task<BaseResponse> UpdatePropertyAsync(int id, CreateMediaProperty dto, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse.Fail("Id düzgün deyil.");

        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken: ct);

        // ✅ Update ediləcək media var?
        var media = await _mediaPropertyRepository.GetByIdAsync(id, ct);
        if (media is null)
            return BaseResponse.Fail($"Id={id} olan Media tapılmadı.");

        // ✅ Yeni PropertyListingId göndərilibsə o property var?
        var propertyExists = await _propertyListingRepository.ExistsByIdAsync(dto.PropertyListingId, ct);
        if (!propertyExists)
            return BaseResponse.Fail($"Id={dto.PropertyListingId} olan Property tapılmadı.");

        // ✅ Mövcud entity üzərinə map et
        _mapper.Map(dto, media);

        _mediaPropertyRepository.Update(media);
        await _mediaPropertyRepository.SaveChangesAsync(ct);

        return BaseResponse.Ok("Media yeniləndi.");
    }
}
