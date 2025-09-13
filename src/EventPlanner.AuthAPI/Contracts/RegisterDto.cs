namespace EventPlanner.AuthAPI.Contracts;

public record RegisterDto(
    string Email,
    string Name,
    string Password);