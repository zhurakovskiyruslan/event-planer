using Microsoft.AspNetCore.Identity;

namespace EventPlanner.AuthAPI.Models;

public class ApplicationUser : IdentityUser<int>
{
    public UserRead Profile { get; set; } = default!;
}



public class UserRead
{
    public int Id { get; set; }              // PK Users
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    // FK -> AspNetUsers(Id), с уникальным индексом
    public int AppUserId { get; set; }

    // Навигация обратно
    public virtual ApplicationUser AppUser { get; set; } = default!;
}