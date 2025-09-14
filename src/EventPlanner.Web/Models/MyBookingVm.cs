namespace EventPlanner.Web.Models;

public record MyBookingVm(
    string EventTitle,
    string EventDescription,
    DateTime EventStartAtUtc,
    string LocationName,
    string LocationAdress,
    string TicketType,
    double TicketPrice,
    string BookingStatus
);