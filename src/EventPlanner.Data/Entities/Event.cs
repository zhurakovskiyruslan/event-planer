namespace EventPlanner.Data.Entities;

public class Event : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartAtUtc { get; set; }
    public int Capacity { get; set; } // максимальное число участников
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public List<Ticket> Tickets { get; private set; } = new();
}