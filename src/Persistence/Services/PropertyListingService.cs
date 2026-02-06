using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Persistence.Services;

public class PropertyListingService : IPropertyListingService
{
    private readonly IPropertyListeningRepository _propertyListingRepository;
    private readonly IMediaPropertyRepository _mediaRepository;
    private readonly IFileStorageService _fileStorage;

    private readonly IMapper _mapper;
    private readonly IValidator<CreatePropertyListing> _createValidator;
    private readonly ICityRepository _cityRepository;
    private readonly IDistrictRepository _districtRepository;

    public PropertyListingService(
        IPropertyListeningRepository propertyListingRepository,
        IMediaPropertyRepository mediaRepository,
        IFileStorageService fileStorage,
        IValidator<CreatePropertyListing> createValidator,
        IMapper mapper,
        ICityRepository cityRepository,
        IDistrictRepository districtRepository)
    {
        _propertyListingRepository = propertyListingRepository;
        _mediaRepository = mediaRepository;
        _fileStorage = fileStorage;

        _createValidator = createValidator;
        _mapper = mapper;
        _cityRepository = cityRepository;
        _districtRepository = districtRepository;
    }

    // ✅ INTERFACE-İN TƏLƏB ETDİYİ CREATE (media ilə)
    public async Task<BaseResponse> CreatePropertyAsync(
        CreatePropertyListing dto,
        List<MediaUploadInput>? media,
        CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken: ct);

        var cityExists = await _cityRepository.ExistsByIdAsync(dto.CityId, ct);
        if (!cityExists)
            return BaseResponse.Fail("Qeyd etdiyiniz City yoxdur");

        var districtExists = await _districtRepository.ExistsByIdAsync(dto.DistrictId, ct);
        if (!districtExists)
            return BaseResponse.Fail("Qeyd etdiyiniz District yoxdur");

        var property = _mapper.Map<PropertyListing>(dto);

        await _propertyListingRepository.AddAsync(property, ct);
        await _propertyListingRepository.SaveChangesAsync(ct);

        // Media varsa: max 5 yoxla, MinIO-ya yüklə, DB-yə yaz
        if (media is { Count: > 0 })
        {
            if (media.Count > 5)
                return BaseResponse.Fail("Maksimum 5 media əlavə etmək olar.");

            var existingCount = await _mediaRepository.CountByPropertyListingIdAsync(property.Id, ct);
            if (existingCount + media.Count > 5)
                return BaseResponse.Fail("Maksimum 5 media allowed for a property.");

            var maxOrder = await _mediaRepository.GetMaxOrderByPropertyListingIdAsync(property.Id, ct);
            var nextOrder = maxOrder <= 0 ? 1 : maxOrder + 1;

            foreach (var m in media)
            {
                var contentType = string.IsNullOrWhiteSpace(m.ContentType)
                    ? "application/octet-stream"
                    : m.ContentType;

                var objectKey = await _fileStorage.SaveAsync(
                    m.Content,
                    m.FileName,
                    contentType,
                    property.Id,
                    ct);

                var entity = new MediaProperty
                {
                    ObjectKey = objectKey,
                    Order = nextOrder++,
                    PropertyListingId = property.Id,
                    MediaType = "image"
                };

                await _mediaRepository.AddAsync(entity, ct);
            }

            await _mediaRepository.SaveChangesAsync(ct);
        }

        return BaseResponse.Ok("Elan yaradıldı.");
    }

    // ✅ INTERFACE-İN TƏLƏB ETDİYİ UPDATE (add/remove media ilə)
    public async Task<BaseResponse> UpdatePropertyAsync(
        int id,
        CreatePropertyListing dto,
        List<MediaUploadInput>? addMedia,
        int[]? removeMediaIds,
        CancellationToken ct)
    {
        if (id <= 0)
            return BaseResponse.Fail("Id düzgün deyil.");

        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken: ct);

        var property = await _propertyListingRepository.GetByIdAsync(id, ct);
        if (property is null)
            return BaseResponse.Fail($"Id={id} olan Property tapılmadı.");

        var cityExists = await _cityRepository.ExistsByIdAsync(dto.CityId, ct);
        if (!cityExists)
            return BaseResponse.Fail("Qeyd etdiyiniz City yoxdur");

        var districtExists = await _districtRepository.ExistsByIdAsync(dto.DistrictId, ct);
        if (!districtExists)
            return BaseResponse.Fail("Qeyd etdiyiniz District yoxdur");

        _mapper.Map(dto, property);
        _propertyListingRepository.Update(property);
        await _propertyListingRepository.SaveChangesAsync(ct);

        // 1) Remove media
        if (removeMediaIds is { Length: > 0 })
        {
            foreach (var mediaId in removeMediaIds)
            {
                var media = await _mediaRepository.GetByIdAsync(mediaId, ct);
                if (media is null) continue;

                // təhlükəsizlik: başqa listing-in mediası silinməsin
                if (media.PropertyListingId != id) continue;

                await _fileStorage.DeleteFileAsync(media.ObjectKey, ct);
                _mediaRepository.Delete(media);
            }

            await _mediaRepository.SaveChangesAsync(ct);
        }

        // 2) Add media
        if (addMedia is { Count: > 0 })
        {
            var existingCount = await _mediaRepository.CountByPropertyListingIdAsync(id, ct);
            if (existingCount + addMedia.Count > 5)
                return BaseResponse.Fail("Maksimum 5 media allowed for a property.");

            var maxOrder = await _mediaRepository.GetMaxOrderByPropertyListingIdAsync(id, ct);
            var nextOrder = maxOrder <= 0 ? 1 : maxOrder + 1;

            foreach (var m in addMedia)
            {
                var contentType = string.IsNullOrWhiteSpace(m.ContentType)
                    ? "application/octet-stream"
                    : m.ContentType;

                var objectKey = await _fileStorage.SaveAsync(
                    m.Content,
                    m.FileName,
                    contentType,
                    id,
                    ct);

                var entity = new MediaProperty
                {
                    ObjectKey = objectKey,
                    Order = nextOrder++,
                    PropertyListingId = id,
                    MediaType = "image"
                };

                await _mediaRepository.AddAsync(entity, ct);
            }

            await _mediaRepository.SaveChangesAsync(ct);
        }

        return BaseResponse.Ok("Elan yeniləndi.");
    }

    public async Task<BaseResponse<List<GetAllPropertyListing>>> GetAllPropertyAsync(CancellationToken ct = default)
    {
        var properties = await _propertyListingRepository.GetAllAsync(ct);
        var data = _mapper.Map<List<GetAllPropertyListing>>(properties);
        return BaseResponse<List<GetAllPropertyListing>>.Ok(data, "Elanlar gətirildi.");
    }

    public async Task<BaseResponse<GetByIdPropertyListing?>> GetByIdPropertyAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse<GetByIdPropertyListing?>.Fail("Id düzgün deyil.");

        var property = await _propertyListingRepository.GetByIdAsync(id, ct);
        if (property is null)
            return BaseResponse<GetByIdPropertyListing?>.Fail($"Id={id} olan Property tapılmadı.");

        var data = _mapper.Map<GetByIdPropertyListing>(property);
        return BaseResponse<GetByIdPropertyListing?>.Ok(data, "Property tapıldı.");
    }

    public async Task<BaseResponse> DeleteByIdPropertyAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse.Fail("Id düzgün deyil.");

        var property = await _propertyListingRepository.GetByIdAsync(id, ct);
        if (property is null)
            return BaseResponse.Fail($"ID-{id} tapılmadı");

        _propertyListingRepository.Delete(property);
        await _propertyListingRepository.SaveChangesAsync(ct);

        return BaseResponse.Ok("Elan silindi");
    }
}
