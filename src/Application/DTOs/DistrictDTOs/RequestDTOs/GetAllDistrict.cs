namespace Application.DTOs.DistrictDTOs.RequestDTOs;

public class GetAllDistrict
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CityId { get; set; }
}
