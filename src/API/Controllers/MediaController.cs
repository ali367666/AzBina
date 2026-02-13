using Application.Abstracts.Services;
using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediaController : ControllerBase
{
    private readonly IMediaPropertyService _mediaPropertyService;

    public MediaController(IMediaPropertyService mediaPropertyService)
    {
        _mediaPropertyService = mediaPropertyService;
    }

    // ✅ Upload (multipart/form-data) - birdən çox şəkil
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] CreateMediaProperty dto, CancellationToken ct)
    {
        var result = await _mediaPropertyService.CreateMediaAsync(dto, ct);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    // GET: api/Media
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediaPropertyService.GetAllMediaAsync(ct);
        return Ok(result);
    }

    // GET: api/Media/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var result = await _mediaPropertyService.GetByIdMediaAsync(id, ct);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    // PUT: api/Media/{id}  (əgər həqiqətən update lazımdırsa)
    /*[HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateMediaProperty dto, CancellationToken ct)
    {
        var result = await _mediaPropertyService.UpdatePropertyAsync(id, dto, ct);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }*/

    // DELETE: api/Media/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var result = await _mediaPropertyService.DeleteByIdMediaAsync(id, ct);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}