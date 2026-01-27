namespace Application.DTOs.DistrictDTOs.RequestDTOs;

public class GetByIdDistrict
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CityId { get; set; }
}
