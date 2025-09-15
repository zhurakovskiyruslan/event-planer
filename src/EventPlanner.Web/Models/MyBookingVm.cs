namespace EventPlanner.Web.Models;

public record MyBookingVm(
    int BookingId,
    int EventId,
    string EventTitle,
    string EventDescription,
    DateTime EventStartAtUtc,
    string LocationName,
    string LocationAdress,
    string TicketType,
    double TicketPrice,
    string BookingStatus
);