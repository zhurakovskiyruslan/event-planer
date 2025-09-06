using EventPlanner.Data.Enums;

namespace EventPlanner.API.Contracts;

public record TicketResponseDto(
    int Id,
    TicketType Type,
    double Price,
    int EventId
    );