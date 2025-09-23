namespace EventPlanner.AuthAPI.Contracts;

public record LoginDto(
    string Email,
    string Password
    );