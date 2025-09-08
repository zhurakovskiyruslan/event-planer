using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Web.Models
{
    public record CreateTicketVm
    {
        [Required(ErrorMessage = "Type is required")]
        [RegularExpression("^(Standard|VIP)$", ErrorMessage = "Type must be either 'Standard' or 'VIP'")]
        public string Type { get; init; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; init; }

        [Required(ErrorMessage = "EventId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "EventId must be greater than 0")]
        public int EventId { get; init; }
    }
}