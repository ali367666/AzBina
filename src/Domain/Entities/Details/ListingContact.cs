using Domain.Entities.Common;
using Domain.Enum;

namespace Domain.Entities.Details;

public class ListingContact : BaseEntity
{
    public ContactRole Role { get; set; }

    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    public int PropertyListingId { get; set; }
    public PropertyListing PropertyListing { get; set; } = null!;
}

