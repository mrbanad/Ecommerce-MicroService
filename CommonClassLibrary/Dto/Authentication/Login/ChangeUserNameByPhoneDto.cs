using System.ComponentModel.DataAnnotations;

namespace CommonClassLibrary.Dto.Authentication.Login;

public class ChangeUserNameByPhoneDto
{
    public string PhoneNumber { get; set; }

    public string Code { get; set; }

    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Confirm UserName is required")]
    [Compare("UserName")]
    public string ConfirmUserName { get; set; }
}