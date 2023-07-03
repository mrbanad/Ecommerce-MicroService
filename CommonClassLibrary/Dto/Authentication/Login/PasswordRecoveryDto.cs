using System.ComponentModel.DataAnnotations;

namespace CommonClassLibrary.Dto.Authentication.Login;

public class PasswordRecoveryDto
{
    public string Email { get; set; }

    public string Token { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 6 and 255 characters", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 6 and 255 characters", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}