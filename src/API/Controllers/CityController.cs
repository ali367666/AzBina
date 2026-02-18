using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CityController : ControllerBase
{
    private readonly ICityService _cityService;

    public CityController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
        => Ok(await _cityService.GetAllCityAsync(ct));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct = default)
    {
        var result = await _cityService.GetByIdCityAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize(Policy = Policies.ManageCities)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCityDTOs dto, CancellationToken ct = default)
    {
        var result = await _cityService.CreateCityAsync(dto, ct);
        return !result.Success ? BadRequest(result) : Ok(result);
    }

    [Authorize(Policy = Policies.ManageCities)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateCityDTOs dto, CancellationToken ct = default)
        => Ok(await _cityService.UpdateCityAsync(id, dto, ct));

    [Authorize(Policy = Policies.ManageCities)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct = default)
        => Ok(await _cityService.DeleteByIdCityAsync(id, ct));
}
