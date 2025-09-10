using FluentValidation;

namespace EventPlanner.Web.Models.Validators;

public class UpsertBookingVmValidator : AbstractValidator<UpsertBookingVm>
{
    public UpsertBookingVmValidator()
    {
        RuleFor(x=>x).NotNull().WithMessage("Booking is required.");
    }
}