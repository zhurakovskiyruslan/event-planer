using FluentValidation;
namespace EventPlanner.Web.Models;

public record RegisterReqVm(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword
    );