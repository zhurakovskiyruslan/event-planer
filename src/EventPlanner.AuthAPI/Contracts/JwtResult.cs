namespace EventPlanner.AuthAPI.Contracts;

public record JwtResult(
    string Token,
    DateTime Expires);