using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Persistence.Services;

public class PropertyListingService : IPropertyListingService
{
    private readonly IPropertyListeningRepository _propertyListingRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePropertyListing> _createValidator;
    private readonly ICityRepository _cityRepository;
    private readonly IDistrictRepository _districtRepository;

    public PropertyListingService(
        IPropertyListeningRepository propertyListingRepository,
        IValidator<CreatePropertyListing> createValidator,
        IMapper mapper,
        ICityRepository cityRepository,
        IDistrictRepository districtRepository)
    {
        _propertyListingRepository = propertyListingRepository;
        _createValidator = createValidator;
        _mapper = mapper;
        _cityRepository = cityRepository;
        _districtRepository = districtRepository;
    }

    public async Task<BaseResponse> CreatePropertyAsync(CreatePropertyListing dto, CancellationToken ct = default)
    {
        // ✅ Validation (səndəki pattern kimi)
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken:ct);


        var cityExists = await _cityRepository.ExistsByIdAsync(dto.CityId, ct);
        if (!cityExists)
            return BaseResponse.Fail("Qeyd etdiyiniz City yoxdur");

        var districtExists=await _districtRepository.ExistsByIdAsync(dto.DistrictId, ct);
        if (!districtExists)
            return BaseResponse.Fail("Qeyd etdiyiniz District yoxdur");

        var property = _mapper.Map<PropertyListing>(dto);

        await _propertyListingRepository.AddAsync(property, ct);
        await _propertyListingRepository.SaveChangesAsync(ct);

        return BaseResponse.Ok("Elan yaradıldı.");
    }



    public async Task<BaseResponse> DeleteByIdPropertyAsync(int id, CancellationToken ct = default)
    {
        if (id < 0)
        {
            return BaseResponse.Fail("Id- duzgun qeyd edilmeyib");
        }
        var media= await _propertyListingRepository.GetByIdAsync(id, ct);
        if (media == null)
        {
            return BaseResponse.Fail($"ID-{id} tapilmadi");
        }
        _propertyListingRepository.Delete(media);
        await _propertyListingRepository.SaveChangesAsync();
        return BaseResponse.Ok("Elan silindi");
    }

    public async Task<BaseResponse<List<GetAllPropertyListing>>> GetAllPropertyAsync(CancellationToken ct = default)
    {
        var properties = await _propertyListingRepository.GetAllAsync(ct);
        var data=_mapper.Map<List<GetAllPropertyListing>>(properties);
        return BaseResponse<List<GetAllPropertyListing>>.Ok(data, "City-lər gətirildi.");
    }

    public async Task<BaseResponse<GetByIdPropertyListing?>>
    GetByIdPropertyAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse<GetByIdPropertyListing?>
                .Fail("Id düzgün deyil.");

        var property = await _propertyListingRepository.GetByIdAsync(id, ct);
        if (property is null)
            return BaseResponse<GetByIdPropertyListing?>
                .Fail($"Id={id} olan Property tapılmadı.");

        var data = _mapper.Map<GetByIdPropertyListing>(property);

        return BaseResponse<GetByIdPropertyListing?>
            .Ok(data, "Property tapıldı.");
    }

    public async Task<BaseResponse> UpdatePropertyAsync(int id, CreatePropertyListing dto, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse.Fail("Id düzgün deyil.");

        // ✅ DTO validation
        await _createValidator.ValidateAndThrowAsync(dto, cancellationToken: ct);

        // ✅ Mövcud entity-ni tap
        var property = await _propertyListingRepository.GetByIdAsync(id, ct);
        if (property is null)
            return BaseResponse.Fail($"Id={id} olan Property tapılmadı.");

        // ✅ City/District mövcuddurmu?
        var cityExists = await _cityRepository.ExistsByIdAsync(dto.CityId, ct);
        if (!cityExists)
            return BaseResponse.Fail("Qeyd etdiyiniz City yoxdur");

        var districtExists = await _districtRepository.ExistsByIdAsync(dto.DistrictId, ct);
        if (!districtExists)
            return BaseResponse.Fail("Qeyd etdiyiniz District yoxdur");

        // ✅ Update (AutoMapper -> mövcud entity üzərinə)
        _mapper.Map(dto, property);

        // Repository EF tracking edirsə, Update çağırmaq şərt deyil,
        // amma pattern olaraq saxlamaq olar:
        _propertyListingRepository.Update(property);

        await _propertyListingRepository.SaveChangesAsync(ct);

        return BaseResponse.Ok("Elan yeniləndi.");
    }

}
