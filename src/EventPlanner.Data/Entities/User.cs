namespace EventPlanner.Data.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Booking> Bookings { get; set; } = new();
}
