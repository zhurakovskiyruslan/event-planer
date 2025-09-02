namespace EventPlanner.Data.Entities;

public class Ticket : BaseEntity
{ 
    public string Type { get; private set; } = "Standard"; // Standard / VIP
    public decimal Price { get; private set; }
    public int EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public List<Booking> Bookings { get; private set; } = new();

    private Ticket() { }

    public Ticket(string type, decimal price, int eventId)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("Type required");
        if (price < 0) throw new ArgumentException("Price must be >= 0");

        Type = type;
        Price = price;
        EventId = eventId;
    }
}