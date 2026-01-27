using Application.Abstracts.Services;
using Application.DTOs.DistrictDTOs.RequestDTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DistrictController : ControllerBase
{
    private readonly IDistrictService _service;
    public DistrictController(IDistrictService service)
    {
        _service = service;

    }
    [HttpGet]
    public async Task<IActionResult> GetAllDistrict(CancellationToken ct = default)
    {
        var districts = await _service.GetAllDistrictAsync(ct);
        return Ok(districts);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdDistrict(int id, CancellationToken ct = default)
    {
        var district = await _service.GetByIdDistrictAsync(id, ct);
        if (district == null)
            return NotFound();
        return Ok(district);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDistrict([FromBody] DistrictCreateDTO dto, CancellationToken ct)
    {
        if (dto == null) return BadRequest();

        var created = await _service.CreateDistrictAsync(dto, ct);
        return Ok(created);
    }

}
