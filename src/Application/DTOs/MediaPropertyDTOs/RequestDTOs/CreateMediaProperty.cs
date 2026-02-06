using Microsoft.AspNetCore.Http;

namespace Application.DTOs.MediaPropertyDTOs.RequestDTOs;

public class CreateMediaProperty
{
    public int PropertyListingId { get; set; }
    public List<IFormFile> Files { get; set; } = new();
}
