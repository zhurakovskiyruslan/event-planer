namespace EventPlanner.Data.Entities;

public class Booking : BaseEntity
{
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public int TicketId { get; private set; }
    public Ticket Ticket { get; private set; } = null!;
    public string Status { get; private set; } = "Active"; // Active / Cancelled
    public Booking(int userId, int ticketId)
    {
        UserId = userId;
        TicketId = ticketId;
        CreatedAt = DateTime.UtcNow;
        Status = "Active";
    }

    public void Cancel()
    {
        if (Status == "Cancelled") throw new InvalidOperationException("Already cancelled");
        Status = "Cancelled";
    }
}