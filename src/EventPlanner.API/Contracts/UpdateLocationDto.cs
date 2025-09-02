namespace EventPlanner.API.Contracts;

public record UpdateLocationDto(
    string Name,
    string Address,
    int Capacity
);