namespace MoviePlatformMonolit.Modules.Users.Domain.DTO.REQUEST;

public class UpdateProfileRequest
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; } 
    public DateTime? DateOfBirth { get; set; }

}