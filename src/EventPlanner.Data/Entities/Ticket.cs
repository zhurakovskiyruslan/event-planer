namespace EventPlanner.Data.Entities;

public class Ticket : BaseEntity
{ 
    public string Type { get; set; } = "Standard"; // Standard / VIP
    public double Price { get; set; }
    public int EventId { get; set; }
    public Event Event { get;  set; } = null!;
    public List<Booking> Bookings { get; private set; } = new();

    public void Update(string type, double price, int eventId)
    {
        if (type != "Standard" && type!= "VIP")
            throw new ArgumentException("Invalid ticket type");
        
        if (Price <= 0)
            throw new ArgumentException("Ticket price must be greater than zero");
        Type = type;
        Price = price;
        EventId = eventId;
    }
}