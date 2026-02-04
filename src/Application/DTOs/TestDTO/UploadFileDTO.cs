using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TestDTO;

public class UploadFileDTO
{
    public IFormFile File { get; set; }
    public string FileName { get; set; }
}
