using EventPlanner.Data.Enums;

namespace EventPlanner.API.Contracts;

public record CreateTicketDto(
    TicketType Type,
    double Price,
    int EventId
    );