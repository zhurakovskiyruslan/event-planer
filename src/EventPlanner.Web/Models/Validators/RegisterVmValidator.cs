using FluentValidation;

namespace EventPlanner.Web.Models.Validators;

public class RegisterVmValidator :  AbstractValidator<RegisterReqVm>
{
    public RegisterVmValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must not exceed 6 characters");
        RuleFor(x=>x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required")
            .Equal(x=>x.Password).WithMessage("Passwords do not match");
        

    }
}