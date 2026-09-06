namespace MoviePLatform_Monolit.Movie.DTO.REQUEST;

public class ReviewRequest
{

    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}