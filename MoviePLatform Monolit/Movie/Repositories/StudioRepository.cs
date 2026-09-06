using MoviePLatform_Monolit.Data;
using MoviePLatform_Monolit.Entity;

namespace MoviePLatform_Monolit.Movie.Repositories;


public interface IStudioRepository : IBaseRepo<StudioEntity> { }

public class StudioRepository : BaseRepository<StudioEntity>, IStudioRepository
{
    public StudioRepository(ApplicationDbContext context) : base(context) { }
}