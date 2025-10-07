namespace EventPlanner.API.Contracts;

public record EventResponseDto(
    int Id,
    string Title,
    string Description,
    DateTime StartAtUtc,
    int Capacity,
    int LocationId
    );