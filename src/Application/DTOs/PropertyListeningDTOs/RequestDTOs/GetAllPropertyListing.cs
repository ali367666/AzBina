using Domain.Enum;

namespace Application.DTOs.PropertyListeningDTOs.RequestDTOs;

public class GetAllPropertyListing
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ListingType ListingType { get; set; }
    public PropertyType PropertyType { get; set; }

    public double Area { get; set; }
    public int Rooms { get; set; }
    public RenovationStatus RenovationStatus { get; set; }

    public int CityId { get; set; }
    public int DistrictId { get; set; }
}
