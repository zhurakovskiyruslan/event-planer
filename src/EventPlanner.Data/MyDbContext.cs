using EventPlanner.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Data;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>() // enum <-> string ("Active"/"Cancelled")
            .HasMaxLength(20);
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Type)
            .HasConversion<string>() // enum <-> string ("Active"/"Cancelled")
            .HasMaxLength(20);
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.Property(x => x.AppUserId).IsRequired();

            // Уникальность: 1:1
            e.HasIndex(x => x.AppUserId).IsUnique();
        });
        
    }
    
   
}