namespace EventPlaner.Data.Entities;

public class Location : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public int Capacity { get; private set; }   // вместимость зала
    public List<Event> Events { get; private set; } = new();

    private Location() { }

    public Location(string name, string address, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required");
        if (capacity <= 0) throw new ArgumentException("Capacity must be > 0");

        Name = name.Trim();
        Address = address.Trim();
        Capacity = capacity;
    }
}