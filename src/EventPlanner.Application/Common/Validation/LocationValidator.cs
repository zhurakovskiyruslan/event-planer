using EventPlanner.Data.Entities;
using FluentValidation;

namespace EventPlanner.Application.Common.Validation;

public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
        {
        RuleFor(x => x).NotNull().WithMessage("Location is required.");
        RuleFor(x=>x.Name).NotEmpty().WithMessage("Location name required.");
        RuleFor(x=>x.Address).NotEmpty().WithMessage("Location address required.");
        RuleFor(x=>x.Capacity).GreaterThan(0).WithMessage("Location capacity must be greater than zero.");
        }
}