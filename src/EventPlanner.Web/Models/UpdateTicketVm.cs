using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Web.Models;

    public record UpdateTicketVm
    {
        [Required, RegularExpression("^(Standard|VIP)$")]
        public string Type { get; init; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; init; }

        [Required, Range(1, int.MaxValue)]
        public int EventId { get; init; }
    }