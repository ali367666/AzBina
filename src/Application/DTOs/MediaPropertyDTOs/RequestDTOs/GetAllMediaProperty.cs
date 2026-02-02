namespace Application.DTOs.MediaPropertyDTOs.RequestDTOs;

public class GetAllMediaProperty
{
    public string MediaUrl { get; set; } = null!;
    public string MediaType { get; set; } = null!;
    public int PropertyListingId { get; set; }
    
}
