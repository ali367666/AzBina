using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyListingController : ControllerBase
{
    private readonly IPropertyListingService _propertyListingService;
    private readonly IMediaPropertyRepository _mediaRepo;
    private readonly IFileStorageService _fileStorage;
    private readonly IMapper _mapper;

    public PropertyListingController(
        IPropertyListingService propertyListingService,
        IMediaPropertyRepository mediaRepo,
        IFileStorageService fileStorage,
        IMapper mapper)
    {
        _propertyListingService = propertyListingService;
        _mediaRepo = mediaRepo;
        _fileStorage = fileStorage;
        _mapper = mapper;
    }

    // POST: api/PropertyListing (multipart/form-data)
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(
        [FromForm] CreatePropertyListing dto,
        IFormFileCollection? media,
        CancellationToken ct)
    {
        List<MediaUploadInput>? mediaInputs = null;

        try
        {
            if (media is { Count: > 0 })
            {
                mediaInputs = media.Select(f => new MediaUploadInput
                {
                    Content = f.OpenReadStream(),
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    Length = f.Length
                }).ToList();
            }

            var result = await _propertyListingService.CreatePropertyAsync(dto, mediaInputs, ct);
            return Ok(result);
        }
        finally
        {
            if (mediaInputs is not null)
                foreach (var m in mediaInputs)
                    m.Content.Dispose();
        }
    }

    // PUT: api/PropertyListing/{id} (multipart/form-data)
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(
        int id,
        [FromForm] CreatePropertyListing dto,
        IFormFileCollection? addMedia,
        int[]? removeMediaIds,
        CancellationToken ct)
    {
        List<MediaUploadInput>? addInputs = null;

        try
        {
            if (addMedia is { Count: > 0 })
            {
                addInputs = addMedia.Select(f => new MediaUploadInput
                {
                    Content = f.OpenReadStream(),
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    Length = f.Length
                }).ToList();
            }

            var result = await _propertyListingService.UpdatePropertyAsync(id, dto, addInputs, removeMediaIds, ct);
            return Ok(result);
        }
        finally
        {
            if (addInputs is not null)
                foreach (var m in addInputs)
                    m.Content.Dispose();
        }
    }

    // POST: api/PropertyListing/{propertyId}/media (tək fayl upload, max 5)
    [HttpPost("{propertyId:int}/media")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadSingleMedia(
        int propertyId,
        IFormFile file, // ✅ [FromForm] YOXDUR!
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is required.");

        var count = await _mediaRepo.CountByPropertyListingIdAsync(propertyId, ct);
        if (count >= 5)
            return BadRequest("Maximum 5 media allowed for a property.");

        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? "application/octet-stream"
            : file.ContentType;

        string objectKey;
        await using (var stream = file.OpenReadStream())
        {
            objectKey = await _fileStorage.SaveAsync(
                stream,
                file.FileName,
                contentType,
                propertyId,
                ct);
        }

        var maxOrder = await _mediaRepo.GetMaxOrderByPropertyListingIdAsync(propertyId, ct);
        var nextOrder = (count == 0) ? 1 : (maxOrder + 1);

        var media = new MediaProperty
        {
            ObjectKey = objectKey,
            Order = nextOrder,
            PropertyListingId = propertyId,
            MediaType = "image"
        };

        await _mediaRepo.AddAsync(media, ct);
        await _mediaRepo.SaveChangesAsync(ct);

        return Ok(new { media.Id, media.ObjectKey, media.Order, media.PropertyListingId });
    }

    // GET: api/PropertyListing/{propertyId}/media
    [HttpGet("{propertyId:int}/media")]
    public async Task<IActionResult> GetMedia(int propertyId, CancellationToken ct)
    {
        var media = await _mediaRepo.GetByPropertyListingIdAsync(propertyId, ct);
        return Ok(media);
    }

    // DELETE: api/PropertyListing/media/{id}
    [HttpDelete("media/{id:int}")]
    public async Task<IActionResult> DeleteMedia(int id, CancellationToken ct)
    {
        var media = await _mediaRepo.GetByIdAsync(id, ct);
        if (media is null)
            return NotFound();

        await _fileStorage.DeleteFileAsync(media.ObjectKey, ct);

        // Səndə repository-də metod adı Delete-dirsə bunu yaz:
        _mediaRepo.Delete(media);
        // Əgər Remove-dursa, yuxarıdakı sətri silib bunu yaz:
        // _mediaRepo.Remove(media);

        await _mediaRepo.SaveChangesAsync(ct);
        return NoContent();
    }
}
