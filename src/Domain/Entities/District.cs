using Domain.Entities.Common;

namespace Domain.Entities;

public class District: BaseEntity<int>
{

    public string Name { get; set; } = null!;
    public int CityId { get; set; }
    public City? City { get; set; } = null!;
}
