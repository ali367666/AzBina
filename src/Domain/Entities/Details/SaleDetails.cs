using Domain.Entities.Common;

namespace Domain.Entities.Details;

public class SaleDetails:BaseEntity<int>
{


    public decimal Price { get; set; }     
    public bool HasMortgage { get; set; }  
    public bool HasExtract { get; set; }  

    public int PropertyListingId { get; set; }
    public PropertyListing PropertyListing { get; set; } = null!;
}

