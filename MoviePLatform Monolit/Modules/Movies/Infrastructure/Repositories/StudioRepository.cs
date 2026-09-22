using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Movie.Repositories;


public interface IStudioRepository : IBaseRepo<StudioEntity> { }

public class StudioRepository : Modules.Movies.Infrastructure.Repositories.BaseRepository<StudioEntity>, IStudioRepository
{
    public StudioRepository(ApplicationDbContext context) : base(context) { }
}