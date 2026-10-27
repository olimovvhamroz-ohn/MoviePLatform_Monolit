
namespace MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;
public class UserResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }


}