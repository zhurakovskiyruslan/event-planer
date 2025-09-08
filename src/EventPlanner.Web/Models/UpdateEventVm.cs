 using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Web.Models;

public record UpdateEventVm(
    [Required, StringLength(100)] string Title,
    [StringLength(200)] string Description,
    [Required] DateTime StartAtUtc,
    [Range(1, 1_000_000)] int Capacity,
    [Range(1, int.MaxValue)] int LocationId);
    