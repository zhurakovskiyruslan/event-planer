namespace EventPlanner.AuthAPI.Contracts;

public record ChangePasswordDto(
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword);