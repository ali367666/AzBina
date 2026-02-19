using Application.DTOs.Recommendations;

namespace Application.Abstracts.Services;

public interface IRecommendationService
{
    Task<ListingRecommendationResponseDto> GetListingInsightsAsync(CancellationToken ct = default);
}
