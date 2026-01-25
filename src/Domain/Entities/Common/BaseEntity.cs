namespace Domain.Entities.Common;

public class BaseEntity<T>
{
    public T ?Id { get; set; }
    public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}
