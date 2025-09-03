namespace EventPlanner.API.Contracts;

public record TicketResponseDto(
    int Id,
    string Type,
    double Price,
    int EventId
    );