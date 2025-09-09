using FluentValidation;
    
namespace EventPlanner.Web.Models.Validators;
    
public class UpsertUsersVmValidator : AbstractValidator<UpsertUserVm>
{
    public UpsertUsersVmValidator()
    {
        RuleFor(x=>x).NotNull().WithMessage("User is required");
        RuleFor(x=>x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x=>x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid");
    }
}