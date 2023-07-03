using System.ComponentModel.DataAnnotations;

namespace CommonClassLibrary.Dto.Authentication.Login;

public class ChangeUserNameDto
{
    public string Email { get; set; }

    public string Code { get; set; }

    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Confirm UserName is required")]
    [Compare("UserName")]
    public string ConfirmUserName { get; set; }
}