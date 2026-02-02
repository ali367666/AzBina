using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;

namespace Application.Abstracts.Services;

public interface IMediaPropertyService
{
    Task<BaseResponse> CreateMediaAsync(CreateMediaProperty dto, CancellationToken ct = default);

    Task<BaseResponse<List<GetAllMediaProperty>>> GetAllMediaAsync(CancellationToken ct = default);

    Task<BaseResponse<GetByIdMediaProperty?>> GetByIdMediaAsync(int id, CancellationToken ct = default);
    Task<BaseResponse> DeleteByIdMediaAsync(int id, CancellationToken ct = default);
    Task<BaseResponse> UpdatePropertyAsync(int id, CreateMediaProperty dto, CancellationToken ct = default);
}
