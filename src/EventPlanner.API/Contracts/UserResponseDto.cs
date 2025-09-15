namespace EventPlanner.API.Contracts;

public record UserResponseDto(
    int Id,
    string Name,
    string Email,
    int AppUserId
    );