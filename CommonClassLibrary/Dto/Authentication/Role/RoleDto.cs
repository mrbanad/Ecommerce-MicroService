namespace CommonClassLibrary.Dto.Authentication.Role;

public class RoleDto : BaseDto
{
    public string Title { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }
}