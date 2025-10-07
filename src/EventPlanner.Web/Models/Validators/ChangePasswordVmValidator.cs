using FluentValidation;

namespace EventPlanner.Web.Models.Validators;

public class ChangePasswordVmValidator : AbstractValidator<ChangePasswordVm>
{
    public ChangePasswordVmValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Old password is required.");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password is required.");
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}