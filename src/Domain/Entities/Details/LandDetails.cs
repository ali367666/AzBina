using Domain.Entities.Common;

namespace Domain.Entities.Details;

public class LandDetails : BaseEntity
{
    public double LandAreaSot { get; set; }

    public int PropertyListingId { get; set; }
    public PropertyListing PropertyListing { get; set; } = null!;
}

