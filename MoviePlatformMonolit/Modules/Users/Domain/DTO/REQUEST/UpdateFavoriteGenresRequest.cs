namespace MoviePlatformMonolit.Modules.Users.Domain.DTO.REQUEST;

public class UpdateFavoriteGenresRequest
{
    public List<long> CategoryIds { get; set; } = new();
}