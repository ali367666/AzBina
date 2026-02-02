using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CityContoller : ControllerBase
{
    private readonly ICityService _cityService;
    public CityContoller(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCity(CancellationToken ct = default)
    {
        var result = await _cityService.GetAllCityAsync(ct);
        return Ok(result);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdCity(int id, CancellationToken ct = default)
    {
        var result = await _cityService.GetByIdCityAsync(id, ct);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityDTOs dto, CancellationToken ct = default)
    {
        var result = await _cityService.CreateCityAsync(dto, ct);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }


    [HttpDelete]
    public async Task<IActionResult> DeleteByIdCity(int id,CancellationToken ct=default)
    {
        var result=await _cityService.DeleteByIdCityAsync(id,ct);
        return Ok(result);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateCity(int id, [FromBody] CreateCityDTOs dto, CancellationToken ct = default)
    {
        if (dto == null)
            return BadRequest();
        var result = await _cityService.UpdateCityAsync(id, dto, ct);
        return Ok(result);
    }
    [HttpGet("{id:int}/districts")]
    public async Task<IActionResult> GetCityWithDistricts(int id, CancellationToken ct)
    {
        var result = await _cityService.GetByIdCityWithDistrictsAsync(id, ct);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }
}
