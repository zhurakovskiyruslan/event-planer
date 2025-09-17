namespace EventPlanner.Web.Models;

public record ChangePasswordReqVm(
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword);