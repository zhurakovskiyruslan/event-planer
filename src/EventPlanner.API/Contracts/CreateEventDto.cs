using System.ComponentModel.DataAnnotations;

namespace EventPlanner.API.Contracts;

public record CreateEventDto
(
    string Title,
    string Description,
    DateTime StartAtUtc,
    int Capacity,
    int LocationId
);