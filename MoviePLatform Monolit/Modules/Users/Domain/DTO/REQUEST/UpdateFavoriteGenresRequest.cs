namespace MoviePLatform_Monolit.Users.DTO.REQUEST;

public class UpdateFavoriteGenresRequest
{
    public List<long> CategoryIds { get; set; } = new();
}