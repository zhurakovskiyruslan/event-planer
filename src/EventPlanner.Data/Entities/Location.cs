namespace EventPlanner.Data.Entities;

public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get;  set; } 
    public List<Event> Events { get; private set; } = new();
}