namespace Application.DTOs.MediaPropertyDTOs.RequestDTOs;

public class CreateMediaProperty
{
    public string MediaUrl { get; set; } = null!;
    public string MediaType { get; set; } = null!;
    public int PropertyListingId { get; set; }
    //public int Order { get; set; }
}
