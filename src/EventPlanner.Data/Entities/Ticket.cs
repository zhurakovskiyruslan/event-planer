using EventPlanner.Data.Enums;

namespace EventPlanner.Data.Entities;

public class Ticket : BaseEntity
{ 
    public TicketType Type { get; set; }  // Standard / VIP
    public double Price { get; set; }
    public int EventId { get; set; }
    public Event Event { get;  set; } = null!;
    public List<Booking> Bookings { get; private set; } = new();
    
}