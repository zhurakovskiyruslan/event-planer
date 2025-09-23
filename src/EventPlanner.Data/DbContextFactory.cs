using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventPlanner.Data;

public class DbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();

        var connectionString = "Host=localhost;Port=5432;Database=EventPlanner;Username=postgres;Password=superuser";

        optionsBuilder.UseNpgsql(connectionString);

        return new MyDbContext(optionsBuilder.Options);
    }
}