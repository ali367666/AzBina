using Domain.Entities.Common;
using Domain.Entities.Details;
using Domain.Enum;

namespace Domain.Entities;

public class PropertyListing : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ListingType ListingType { get; set; }
    public PropertyType PropertyType { get; set; }

    public double Area { get; set; }
    public int Rooms { get; set; }
    public RenovationStatus RenovationStatus { get; set; }

    public ICollection<MediaProperty> MediaProperties { get; set; } = new List<MediaProperty>();

    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public int DistrictId { get; set; }
    public District District { get; set; } = null!;

    // ListingType-a görə (satış/kirayə)
    public SaleDetails? SaleDetails { get; set; }
    public RentDetails? RentDetails { get; set; }

    // PropertyType-a görə (mənzil/ev/torpaq)
    public ApartmentDetails? ApartmentDetails { get; set; }
    public HouseDetails? HouseDetails { get; set; }
    public LandDetails? LandDetails { get; set; }

    // Login yoxdur deyə: hər elanın kontaktı MÜTLƏQ olmalıdır
    public ListingContact Contact { get; set; } = null!;
}


