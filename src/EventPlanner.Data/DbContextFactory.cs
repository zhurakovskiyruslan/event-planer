using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventPlanner.Data;

/// <summary>
/// Design-time фабрика, чтобы dotnet-ef мог создать DbContext без запуска WebApi.
/// </summary>
public class DbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();

        // Строка подключения к PostgreSQL
        var connectionString = "Host=localhost;Port=5432;Database=EventPlanner;Username=postgres;Password=superuser";

        optionsBuilder.UseNpgsql(connectionString);

        return new MyDbContext(optionsBuilder.Options);
    }
}
