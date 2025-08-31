using EventPlaner.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPlaner.Data;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
        
    }
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<Ticket>  Tickets { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    
    
}