using EventPlanner.Data.Entities;
using FluentValidation;

namespace EventPlanner.Application.Common.Validation;

public class BookingValidator : AbstractValidator<Booking>
{
    public BookingValidator()
    {
        RuleFor(x=>x).NotNull().WithMessage("Booking is required.");
    }
}