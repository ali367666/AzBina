using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;

namespace Application.Abstracts.Services;

public interface IPropertyListingService
{
    Task<BaseResponse> CreatePropertyAsync(
        CreatePropertyListing dto,
        List<MediaUploadInput>? media,
        int userId,
        CancellationToken ct);
    Task<BaseResponse<List<GetAllPropertyListing>>> GetAllPropertyAsync(CancellationToken ct = default);

    Task<BaseResponse<GetByIdPropertyListing?>> GetByIdPropertyAsync(int id, CancellationToken ct = default);
    Task<BaseResponse> DeleteByIdPropertyAsync(int id, CancellationToken ct = default);
    Task<BaseResponse> UpdatePropertyAsync(int id, CreatePropertyListing dto, List<MediaUploadInput>? addMedia, int[]? removeMediaIds, CancellationToken ct);
    

}
