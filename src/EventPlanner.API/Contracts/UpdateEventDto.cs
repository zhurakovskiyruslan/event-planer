namespace EventPlanner.API.Contracts;

public record UpdateEventDto( 
    string Title,
    string Description,
    DateTime StartAtUtc,
    int Capacity,
    int LocationId
    );