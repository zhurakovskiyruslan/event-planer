namespace EventPlanner.Web.Models;

public record TicketBookVm
(
    int TicketId,
    string TicketType,
    double Price,
    int EventId,
    string EventTitle,
    string EventDescription,
    DateTime EventStartAtUtc,
    string EventLocation,
    string LocationAddress
);