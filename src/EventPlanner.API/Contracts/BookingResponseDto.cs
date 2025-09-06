namespace EventPlanner.API.Contracts;

public record BookingResponseDto(
    int Id,
    int UserId,
    string UserName,
    int TicketId,
    string Status
    );