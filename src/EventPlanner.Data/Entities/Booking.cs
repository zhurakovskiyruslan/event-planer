using EventPlanner.Data.Enums;

namespace EventPlanner.Data.Entities;

public class Booking : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public BookingStatus Status { get; set; } = BookingStatus.Active; // Active / Cancelled
}