namespace Application.DTOs.Recommendations;

public sealed class ListingRecommendationItemDto
{
    public string Region { get; set; } = string.Empty;
    public int ListingCount { get; set; }
    public decimal AveragePrice { get; set; }
}
