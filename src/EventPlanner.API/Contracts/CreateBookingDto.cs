namespace EventPlanner.API.Contracts;

public record CreateBookingDto(
    int UserId,
    int TicketId
    );