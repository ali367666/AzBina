using Application.Abstracts.Services;
using Application.DTOs.TestDTO;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IUploadFileService _service;

    public FileUploadController(IUploadFileService service)
    {
        _service = service;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileDTO dto, CancellationToken ct)
    {
        try
        {
            var entity = await _service.UploadAsync(dto.File, dto.FileName, ct);
            return Created(entity.FileUrl, entity);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var entity = await _service.GetAsync(id, ct);
        if (entity == null) return NotFound();
        return Ok(entity);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var list = await _service.GetAllAsync(ct);
        return Ok(list);
    }
}
