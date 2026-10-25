namespace MoviePLatform_Monolit.Modules.Users.Domain.DTO.REQUEST;

public class UpdateFavoriteGenresRequest
{
    public List<long> CategoryIds { get; set; } = new();
}