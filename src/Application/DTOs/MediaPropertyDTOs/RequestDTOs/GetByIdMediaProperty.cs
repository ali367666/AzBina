namespace Application.DTOs.MediaPropertyDTOs.RequestDTOs;

public class GetByIdMediaProperty
{
    public int Id { get; set; }
    public string MediaUrl { get; set; } = null!;
    public string MediaType { get; set; } = null!;
    public int PropertyListingId { get; set; }
   // public int Order { get; set; }
}
