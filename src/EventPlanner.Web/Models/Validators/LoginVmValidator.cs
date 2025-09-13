using FluentValidation;
using FluentValidation.AspNetCore;

namespace EventPlanner.Web.Models.Validators;

public class LoginVmValidator :  AbstractValidator<LoginReqVm>
{
    public LoginVmValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}