namespace EventPlanner.API.Contracts;

public record CreateTicketDto(
    string Type,
    double Price,
    int EventId
    );