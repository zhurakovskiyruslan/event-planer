namespace EventPlanner.API.Contracts;

public record UpdateTicketDto(
    string Type,
    double Price,
    int EventId
    );