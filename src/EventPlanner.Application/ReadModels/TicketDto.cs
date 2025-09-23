using EventPlanner.Data.Enums;

namespace EventPlanner.Application.ReadModels;

public record TicketDto(
    int TicketId,
    TicketType TicketType,
    double Price,
    int EventId,
    string EventTitle,
    string EventDescription,
    DateTime EventStartAtUtc,
    string EventLocation,
    string LocationAddress
    );