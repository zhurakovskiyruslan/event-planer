namespace EventPlanner.Web.Models;

public record ChangePasswordVm(
    string OldPassword,
    string NewPassword,
    string ConfirmPassword
    );