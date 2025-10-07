using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Web.Models
{
    public record UpsertTicketVm
    {
        public string Type { get; init; } = string.Empty;  // "Standard" | "VIP"
        public double Price { get; init; }
        public int EventId { get; init; }
    }
}