namespace EventPlanner.Web.Models;

public record ChangePasswordReqVm(
    int Id,
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword);