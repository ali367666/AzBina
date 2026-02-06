using System.IO;

namespace Application.DTOs.MediaPropertyDTOs.RequestDTOs;

public sealed class MediaUploadInput
{
    public required Stream Content { get; init; }
    public required string FileName { get; init; }
    public string? ContentType { get; init; }
    public long Length { get; init; }
}
