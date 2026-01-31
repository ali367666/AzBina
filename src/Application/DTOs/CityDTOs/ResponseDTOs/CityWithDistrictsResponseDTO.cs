namespace Application.DTOs.CityDTOs.ResponseDTOs;

public class CityWithDistrictsResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<string> DistrictNames { get; set; } = new();
}
