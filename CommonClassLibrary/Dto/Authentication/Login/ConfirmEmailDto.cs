namespace CommonClassLibrary.Dto.Authentication.Login;

public class ConfirmEmailDto
{
    public ConfirmEmailDto(string token, string email)
    {
        Token = token;
        Email = email;
    }

    public string Token { get; set; }

    public string Email { get; set; }
}