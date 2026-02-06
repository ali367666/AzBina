using Domain.Entities.Common;

namespace Domain.Entities;

public class MediaProperty: BaseEntity<int>
{
    public string ObjectKey { get; set; } = default!;  // MinIO object adı (guid.jpg)
    public int Order { get; set; }

    public int PropertyListingId { get; set; }
    public PropertyListing PropertyListing { get; set; } = default!;

    // İstəsən saxla (tələb etmirsə silmək olar)
    public string? MediaType { get; set; } // "image"
}
