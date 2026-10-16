using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;


public interface IStudioRepository : IBaseRepo<StudioEntity> { }

public class StudioRepository : BaseRepository<StudioEntity>, IStudioRepository
{
    public StudioRepository(ApplicationDbContext context) : base(context) { }
}