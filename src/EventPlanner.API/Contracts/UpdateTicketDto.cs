using EventPlanner.Data.Enums;

namespace EventPlanner.API.Contracts;

public record UpdateTicketDto(
    TicketType Type,
    double Price,
    int EventId
    );