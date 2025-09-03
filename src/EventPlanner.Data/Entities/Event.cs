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

    public void Update(string title, string description, DateTime startAtUtc, int capacity, int locationId)
    {
        if (string.IsNullOrEmpty(title))
            throw new ArgumentException("Title required");
        if (string.IsNullOrEmpty(description))
            throw new ArgumentException("Description required");
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be > 0");
        if (startAtUtc < DateTime.UtcNow)
            throw new ArgumentException("Start date must be after start date");
        Title = title;
        Description = description;
        StartAtUtc = startAtUtc;
        Capacity = capacity;
        LocationId = locationId;
    }

}