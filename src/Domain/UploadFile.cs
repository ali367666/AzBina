using Domain.Entities.Common;

namespace Domain;

public class UploadFile:BaseEntity<int>
{
    public string? FileName { get; set; }
    public string FileUrl { get; set; }
}
