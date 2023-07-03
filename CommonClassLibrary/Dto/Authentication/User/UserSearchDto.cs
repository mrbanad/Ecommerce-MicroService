namespace CommonClassLibrary.Dto.Authentication.User;

public class UserSearchDto : BaseSearchDto
{
    public string? Title { get; set; }

    public string? PhoneNumber { get; set; }
}