namespace EventPlanner.API.Contracts;

public record CreateLocationDto(
    string Name,
    string Address,
    int Capacity
);