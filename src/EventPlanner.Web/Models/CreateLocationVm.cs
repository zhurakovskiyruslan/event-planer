using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Web.Models;

public record CreateLocationVm(
    [Required, StringLength(100)] string Name,
    [Required, StringLength(200)] string Address,
    [Range(1, 1_000_000)] int Capacity);