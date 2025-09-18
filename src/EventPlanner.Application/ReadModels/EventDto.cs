namespace EventPlanner.Application.ReadModels;

public record EventDto(
    int Id,
    string Title,
    string Description,
    DateTime StartAtUtc,
    int Capacity,
    int LocationId,
    string LocationTitle,
    int AvailableSeats
);