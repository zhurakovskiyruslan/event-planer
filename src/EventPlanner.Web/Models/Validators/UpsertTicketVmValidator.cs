using FluentValidation;

namespace EventPlanner.Web.Models.Validators;

public class UpsertTicketVmValidator : AbstractValidator<UpsertTicketVm>
{
    public UpsertTicketVmValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required")
            .Must(t => t == "Standard" || t == "VIP")
            .WithMessage("Type must be either 'Standard' or 'VIP'");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.EventId)
            .GreaterThan(0).WithMessage("EventId must be greater than 0");
    }
}