namespace EventPlanner.AuthAPI.Contracts;

public record RegisterDto(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword);