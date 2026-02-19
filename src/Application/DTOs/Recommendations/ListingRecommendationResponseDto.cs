namespace Application.DTOs.Recommendations;

public sealed class ListingRecommendationResponseDto
{
    public string Message { get; set; } = string.Empty;
    public ListingRecommendationItemDto? CheapestRegion { get; set; }
    public List<ListingRecommendationItemDto> MostActiveRegions { get; set; } = new();
}
