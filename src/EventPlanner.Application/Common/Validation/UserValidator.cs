using EventPlanner.Data.Entities;
using FluentValidation;

namespace EventPlanner.Application.Common.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x=>x).NotNull().WithMessage("User is required");
        RuleFor(x=>x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x=>x.Email).NotEmpty().WithMessage("Email is required")
                                .EmailAddress().WithMessage("Email is invalid");
    }
}