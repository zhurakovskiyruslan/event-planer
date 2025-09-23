// AuthAPI/Data/AppIdentityDbContext.cs

using EventPlanner.AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.AuthAPI.Data;

public class AppIdentityDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }

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
    }
}