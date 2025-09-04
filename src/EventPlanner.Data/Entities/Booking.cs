namespace EventPlanner.Data.Entities;

public class Booking : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public string Status { get; set; } = "Active"; // Active / Cancelled
    
    public void Cancel()
    {
        if (Status == "Cancelled") throw new InvalidOperationException("Already cancelled");
        Status = "Cancelled";
    }
}