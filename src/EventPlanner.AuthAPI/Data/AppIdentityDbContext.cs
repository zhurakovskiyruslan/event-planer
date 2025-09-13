// AuthAPI/Data/AppIdentityDbContext.cs
using EventPlanner.AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.AuthAPI.Data;

public class AppIdentityDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    // ⚠️ Переименовано, чтобы не конфликтовать с IdentityDbContext.Users
    public DbSet<UserRead> DomainUsers => Set<UserRead>();

    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
        builder.Entity<ApplicationRole>().ToTable("AspNetRoles");
        builder.Entity<IdentityUserRole<int>>().ToTable("AspNetUserRoles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("AspNetUserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("AspNetUserLogins");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("AspNetRoleClaims");
        builder.Entity<IdentityUserToken<int>>().ToTable("AspNetUserTokens");

        builder.Entity<UserRead>(e =>
        {
            // Если таблицу "Users" создаёт/мигрирует основной Data-проект:
            e.ToTable("Users", t => t.ExcludeFromMigrations());

            e.HasKey(u => u.Id);

            // строгий FK 1:1 через AppUserId
            e.Property(u => u.AppUserId).IsRequired();

            e.HasOne(u => u.AppUser)
                .WithOne(a => a.Profile)
                .HasForeignKey<UserRead>(u => u.AppUserId)   // FK -> AspNetUsers(Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(u => u.AppUserId).IsUnique();       // уникальность => 1:1
        });
    }
}