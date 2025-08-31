namespace EventPlaner.Data.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public List<Booking> Bookings { get; private set; } = new();

    private User() { }

    public User(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email required");

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
    }
}
