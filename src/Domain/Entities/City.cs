using Domain.Entities.Common;

namespace Domain.Entities;

public class City:BaseEntity
{
    public string Name { get; set; }=null!;
    public ICollection<District> Districts { get; set; } = new List<District>();

}
