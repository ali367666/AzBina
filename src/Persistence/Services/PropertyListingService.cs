using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using Application.Options;
using Application.Shared.Helpers.Responses;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using IEmailSender = Application.Abstracts.Services.IEmailSender;

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

    private readonly IEmailSender _emailSender;
    private readonly UserManager<User> _userManager;
    private readonly EmailOptions _emailOptions;

    

    public PropertyListingService(
        IPropertyListeningRepository propertyListingRepository,
        IMediaPropertyRepository mediaRepository,
        IFileStorageService fileStorage,
        IValidator<CreatePropertyListing> createValidator,
        IMapper mapper,
        ICityRepository cityRepository,
        IDistrictRepository districtRepository,
        IEmailSender emailSender,
        UserManager<User> userManager,
        IOptions<EmailOptions> emailOptions)
    {
        _propertyListingRepository = propertyListingRepository;
        _mediaRepository = mediaRepository;
        _fileStorage = fileStorage;

        _createValidator = createValidator;
        _mapper = mapper;
        _cityRepository = cityRepository;
        _districtRepository = districtRepository;

        _emailSender = emailSender;
        _userManager = userManager;
        _emailOptions = emailOptions.Value;
    }

    // ✅ CREATE (media ilə)
    public async Task<BaseResponse> CreatePropertyAsync(
    CreatePropertyListing dto,
    List<MediaUploadInput>? media,
    int userId,
    CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken: ct);

        var cityExists = await _cityRepository.ExistsByIdAsync(dto.CityId, ct);
        if (!cityExists)
            return BaseResponse.Fail("Qeyd etdiyiniz City yoxdur");

        var districtExists = await _districtRepository.ExistsByIdAsync(dto.DistrictId, ct);
        if (!districtExists)
            return BaseResponse.Fail("Qeyd etdiyiniz District yoxdur");

        // ✅ DTO -> PropertyListing (sadəcə FK-lar + əsas datalar)
        var property = _mapper.Map<PropertyListing>(dto);

        // ✅ Bu elanı kim yaratdı?
        property.UserId = userId;
        property.CreatedAt = DateTime.UtcNow;

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

        // ✅ City/District adlarını DB-dən oxu (mapper YOX!)
        var cityName = await _cityRepository.GetNameByIdAsync(dto.CityId, ct);
        var districtName = await _districtRepository.GetNameByIdAsync(dto.DistrictId, ct);

        // ✅ Elan yaradıldı → elanı yaradan user-ə mail get
        await SendPropertyCreatedEmailToOwnerAsync(property, cityName, districtName, userId, ct);

        return BaseResponse.Ok("Elan yaradıldı.");
    }

    private async Task SendPropertyCreatedEmailToOwnerAsync(
        PropertyListing property,
        string? cityName,
        string? districtName,
        int userId,
        CancellationToken ct)
    {
        if (!_emailOptions.Enabled) return;

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return;

        var toEmail = user.Email;
        if (string.IsNullOrWhiteSpace(toEmail)) return;

        var subject = "Elanınız uğurla yaradıldı";

        var safeCity = System.Net.WebUtility.HtmlEncode(cityName ?? property.CityId.ToString());
        var safeDistrict = System.Net.WebUtility.HtmlEncode(districtName ?? property.DistrictId.ToString());

        var html = $@"
        <h2>Elan yaradıldı ✅</h2>
        <p>Salam {System.Net.WebUtility.HtmlEncode(user.FullName ?? user.UserName ?? "")},</p>
        <p>Elanınız aşağıdakı məlumatlarla yaradıldı:</p>
        <hr/>
        <table style='border-collapse:collapse;'>
            <tr><td style='padding:4px 10px;'><b>Elan ID:</b></td><td style='padding:4px 10px;'>{property.Id}</td></tr>
            <tr><td style='padding:4px 10px;'><b>Şəhər:</b></td><td style='padding:4px 10px;'>{safeCity}</td></tr>
            <tr><td style='padding:4px 10px;'><b>Rayon:</b></td><td style='padding:4px 10px;'>{safeDistrict}</td></tr>
            <tr><td style='padding:4px 10px;'><b>Tarix (UTC):</b></td><td style='padding:4px 10px;'>{property.CreatedAt:yyyy-MM-dd HH:mm}</td></tr>
        </table>
        <p>Uğurlar!</p>
    ";

        var text = $@"
Elan yaradıldı ✅
Salam {user.FullName ?? user.UserName},

Elanınız aşağıdakı məlumatlarla yaradıldı:
Elan ID: {property.Id}
Şəhər: {cityName ?? property.CityId.ToString()}
Rayon: {districtName ?? property.DistrictId.ToString()}
Tarix (UTC): {property.CreatedAt:yyyy-MM-dd HH:mm}

Uğurlar!
";

        try
        {
            await _emailSender.SendAsync(toEmail, subject, html, text, ct);
        }
        catch
        {
            // log yazmaq yaxşıdır (email fail olsa da create fail olmasın)
        }
    }


    // ✅ UPDATE (səndə olduğu kimi saxladım)
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
