using FluentValidation;

namespace EventPlanner.Web.Models.Validators;

public class UpsertLocationVmValidator : AbstractValidator<LocationVm>
{
    public UpsertLocationVmValidator()
    {
        {
            RuleFor(x => x).NotNull().WithMessage("Location is required.");
            RuleFor(x=>x.Name).NotEmpty().WithMessage("Location name required.");
            RuleFor(x=>x.Address).NotEmpty().WithMessage("Location address required.");
            RuleFor(x=>x.Capacity).GreaterThan(0).WithMessage("Location capacity must be greater than zero.");
        }
    }
}