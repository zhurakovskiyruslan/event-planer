namespace EventPlanner.Application.ReadModels;

public record BookingDto(
    string EventTitle,
    string EventDescription,
    DateTime EventStartAtUtc,
    string LocationName,
    string LocationAdress,
    string TicketType,
    double TicketPrice,
    string BookingStatus);