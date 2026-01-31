using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;

namespace Application.Abstracts.Services;

public interface IPropertyListingService
{
    Task<BaseResponse> CreatePropertyAsync(CreatePropertyListing dto, CancellationToken ct = default);

    Task<BaseResponse<List<GetAllPropertyListing>>> GetAllPropertyAsync(CancellationToken ct = default);

    Task<BaseResponse<GetByIdPropertyListing?>> GetByIdPropertyAsync(int id, CancellationToken ct = default);
    Task<BaseResponse> DeleteByIdPropertyAsync(int id, CancellationToken ct = default);
    Task<BaseResponse> UpdatePropertyAsync(int id, CreatePropertyListing dto, CancellationToken ct = default);
    
}
