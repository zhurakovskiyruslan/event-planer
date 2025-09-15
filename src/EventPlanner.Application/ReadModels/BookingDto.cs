namespace EventPlanner.Application.ReadModels;

public record BookingDto(
    int BookingId,
    int EventId,
    string EventTitle,
    string EventDescription,
    DateTime EventStartAtUtc,
    string LocationName,
    string LocationAddress,
    string TicketType,
    double TicketPrice,
    string BookingStatus);