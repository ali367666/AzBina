using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
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



    public Task<BaseResponse> DeleteByIdPropertyAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<List<GetAllPropertyListing>>> GetAllPropertyAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<GetByIdPropertyListing?>> GetByIdPropertyAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse> UpdatePropertyAsync(int id, CreatePropertyListing dto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
