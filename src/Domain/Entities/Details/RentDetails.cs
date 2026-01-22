using Domain.Enum;

namespace Domain.Entities.Details;

public class RentDetails
{
    public int Id { get; set; }

    public RentType RentType { get; set; }  // Günlük / Aylıq
    public decimal Price { get; set; }

    public int PropertyListingId { get; set; }
    public PropertyListing PropertyListing { get; set; } = null!;
}

