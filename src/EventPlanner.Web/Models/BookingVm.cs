namespace EventPlanner.Web.Models;

public record BookingVm(
    int Id,
    int UserId,
    string UserName,
    int TicketId,
    string Status);