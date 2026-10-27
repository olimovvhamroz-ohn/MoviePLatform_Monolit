namespace MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

public class StudioResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int FoundYear { get; set; }

}