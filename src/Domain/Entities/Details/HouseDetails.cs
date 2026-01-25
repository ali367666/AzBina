using Domain.Entities.Common;

namespace Domain.Entities.Details;

public class HouseDetails : BaseEntity<int>
{
    public double LandAreaSot { get; set; }
    public int? HouseFloors { get; set; } 
    public int PropertyListingId { get; set; }
    public PropertyListing PropertyListing { get; set; } = null!;
}

