namespace EventPlanner.Data.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Booking> Bookings { get; set; } = new();

    public void Update(string name, string email)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be empty");
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email cannot be empty");
        Name = name;
        Email = email;
    }
}
