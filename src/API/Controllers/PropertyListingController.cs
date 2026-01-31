using Application.Abstracts.Services;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyListingController : ControllerBase
{
    private readonly IPropertyListingService _propertyListingService;

    public PropertyListingController(IPropertyListingService propertyListingService)
    {
        _propertyListingService = propertyListingService;
    }

    // POST: api/PropertyListing
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePropertyListing dto,
        CancellationToken ct)
    {
        var result = await _propertyListingService.CreatePropertyAsync(dto, ct);
        return Ok(result);
    }

    // GET: api/PropertyListing
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _propertyListingService.GetAllPropertyAsync(ct);
        return Ok(result);
    }

    // GET: api/PropertyListing/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken ct)
    {
        var result = await _propertyListingService.GetByIdPropertyAsync(id, ct);
        return Ok(result);
    }

    // PUT: api/PropertyListing/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CreatePropertyListing dto,
        CancellationToken ct)
    {
        var result = await _propertyListingService.UpdatePropertyAsync(id, dto, ct);
        return Ok(result);
    }

    // DELETE: api/PropertyListing/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var result = await _propertyListingService.DeleteByIdPropertyAsync(id, ct);
        return Ok(result);
    }
}
