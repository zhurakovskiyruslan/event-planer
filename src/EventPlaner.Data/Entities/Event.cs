namespace EventPlaner.Data.Entities;

public class Event : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime StartAtUtc { get; private set; }
    public int Capacity { get; private set; }   // максимальное число участников
    public int LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public List<Ticket> Tickets { get; private set; } = new();
}