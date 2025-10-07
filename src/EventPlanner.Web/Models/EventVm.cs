namespace EventPlanner.Web.Models;

public record EventVm(
    int Id, 
    string Title,
    string Description,
    DateTime StartAtUtc,
    int Capacity,
    int LocationId,
    string LocationTitle,
    int AvailableSeats
    );