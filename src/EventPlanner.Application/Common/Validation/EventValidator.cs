using EventPlanner.Data.Entities;
using FluentValidation;

namespace EventPlanner.Application.Common.Validation;

public class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(x=> x).NotNull().WithMessage("Event is required");
        RuleFor(x=>x.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(x=>x.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(x=>x.StartAtUtc).GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future");
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("Capacity must be greater than 0");
    }
}