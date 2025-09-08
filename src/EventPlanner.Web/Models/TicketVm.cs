namespace EventPlanner.Web.Models;

public record TicketVm(
    int Id,
    string Type,
    double Price,
    int EventId
);