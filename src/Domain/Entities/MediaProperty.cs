using Domain.Entities.Common;

namespace Domain.Entities;

public class MediaProperty: BaseEntity<int>
{
    public string MediaUrl { get; set; } = null!;
    public string MediaType { get; set; } = null!;
    public int PropertyListingId { get; set; }
    public int Order { get; set; }
    public PropertyListing PropertyListing { get; set; } = null!;
}
