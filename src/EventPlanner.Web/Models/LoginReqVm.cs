using FluentValidation;
namespace EventPlanner.Web.Models;

public record LoginReqVm(string Email, string Password);