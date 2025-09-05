using EventPlanner.Data.Enums;

namespace EventPlanner.Data.Entities;

public class Ticket : BaseEntity
{ 
    public TicketType Type { get; set; }  // Standard / VIP
    public double Price { get; set; }
    public int EventId { get; set; }
    public Event Event { get;  set; } = null!;
    public List<Booking> Bookings { get; private set; } = new();

    public void Update(string type, double price, int eventId)
    {
        if (Price <= 0)
            throw new ArgumentException("Ticket price must be greater than zero");
        switch (type)
        {
            case "Standard":
                Type = TicketType.Standard;
                break;
            case "VIP":
                Type = TicketType.VIP;
                break;
            default: 
                throw new ArgumentException("Ticket type must be one of: Standard, VIP");
        }
        Price = price;
        EventId = eventId;
    }
}