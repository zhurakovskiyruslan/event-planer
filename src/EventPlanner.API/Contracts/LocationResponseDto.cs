namespace EventPlanner.API.Contracts;

public record LocationResponseDto(
    int Id,
    string Name,
    string Address,
    int Capacity
);