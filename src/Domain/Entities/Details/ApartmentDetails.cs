using Domain.Entities.Common;

namespace Domain.Entities.Details;

public class ApartmentDetails : BaseEntity
{
    public int Floor { get; set; }         
    public int TotalFloors { get; set; }   

    public int PropertyListingId { get; set; }
    public PropertyListing PropertyListing { get; set; } = null!;
}

