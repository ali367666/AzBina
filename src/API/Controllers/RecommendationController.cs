using Application.Abstracts.Services;
using Application.Shared.Helpers.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpGet("listing-insights")]
    public async Task<IActionResult> GetListingInsights(CancellationToken ct)
    {
        var result = await _recommendationService.GetListingInsightsAsync(ct);
        return Ok(BaseResponse.Ok(result));
    }
}
