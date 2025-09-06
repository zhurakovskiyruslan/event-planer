namespace EventPlanner.API.Contracts;

public record CreateUserDto(
    string Name,
    string Email
    );