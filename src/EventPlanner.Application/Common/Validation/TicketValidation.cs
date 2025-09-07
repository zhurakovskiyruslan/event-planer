using EventPlanner.Data.Entities;
using FluentValidation;

namespace EventPlanner.Application.Common.Validation;

public class TicketValidation : AbstractValidator<Ticket>
{
    public TicketValidation()
    {
        RuleFor(x=>x).NotNull().WithMessage("Ticket is required");
        RuleFor(x => x.Type).IsInEnum().WithMessage("invalid ticket type");
        RuleFor(x=>x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}