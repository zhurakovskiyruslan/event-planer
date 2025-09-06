namespace EventPlanner.Data.Entities;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }
}