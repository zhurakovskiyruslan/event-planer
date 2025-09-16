namespace EventPlanner.AuthAPI.Contracts;

public record ChangePasswordDto(
    int Id,
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword
    );