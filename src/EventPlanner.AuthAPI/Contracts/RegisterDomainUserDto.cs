namespace EventPlanner.AuthAPI.Contracts;

public record CreateDomainUserDto(
    string Name,
    string Email,
    int AppUserId);