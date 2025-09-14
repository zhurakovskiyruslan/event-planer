namespace EventPlanner.Application.ReadModels;

public record EventDto(
    int Id,
    string Title,
    string Description,
    DateTime StartAtUtc,
    int Capacity,
    string LocationTitle
);