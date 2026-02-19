using Application.Abstracts.Services;
using Application.DTOs.Recommendations;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Services;

public sealed class RecommendationService : IRecommendationService
{
    private readonly BinaDbContext _context;

    public RecommendationService(BinaDbContext context)
    {
        _context = context;
    }

    public async Task<ListingRecommendationResponseDto> GetListingInsightsAsync(CancellationToken ct = default)
    {
        var listingData = await _context.PropertyListings
            .AsNoTracking()
            .Select(x => new
            {
                Region = x.City.Name + " / " + x.District.Name,
                Price = (decimal?)(x.SaleDetails != null ? x.SaleDetails.Price : x.RentDetails != null ? x.RentDetails.Price : 0)
            })
            .ToListAsync(ct);

        if (listingData.Count == 0)
        {
            return new ListingRecommendationResponseDto
            {
                Message = "Hazırda analiz üçün elan yoxdur."
            };
        }

        var grouped = listingData
            .GroupBy(x => x.Region)
            .Select(g => new ListingRecommendationItemDto
            {
                Region = g.Key,
                ListingCount = g.Count(),
                AveragePrice = g.Where(v => v.Price.HasValue).Select(v => v.Price!.Value).DefaultIfEmpty(0m).Average()
            })
            .OrderByDescending(x => x.ListingCount)
            .ToList();

        var topRegions = grouped.Take(3).ToList();
        var cheapestRegion = grouped.OrderBy(x => x.AveragePrice).FirstOrDefault();

        return new ListingRecommendationResponseDto
        {
            Message = "Ən aktiv regionlar və orta qiymətə görə ən sərfəli region hesablandı.",
            MostActiveRegions = topRegions,
            CheapestRegion = cheapestRegion
        };
    }
}
