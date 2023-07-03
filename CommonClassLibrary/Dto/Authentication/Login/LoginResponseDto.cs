namespace CommonClassLibrary.Dto.Authentication.Login;

public class LoginResponseDto
{
    public string Token { get; set; }

    public DateTime ExpireDate { get; set; }
}