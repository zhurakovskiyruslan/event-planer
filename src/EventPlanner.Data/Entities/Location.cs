namespace EventPlanner.Data.Entities;

public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get;  set; }   // вместимость зала
    public List<Event> Events { get; private set; } = new();
    
    public void Update(string name, string address, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name required");
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be > 0");

        Name = name.Trim();
        Address = address.Trim();
        Capacity = capacity;
    }
}