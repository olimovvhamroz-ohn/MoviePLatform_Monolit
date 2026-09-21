using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;


namespace MoviePLatform_Monolit.Movie.Repositories;

public interface IActorRepository : IBaseRepo<ActorEntity>
{
    Task<ActorEntity>GetByName(string name);
    Task<ActorEntity?> FindByName(string name);
}
public class ActorRepository : BaseRepository<ActorEntity>, IActorRepository
{
    public ActorRepository(ApplicationDbContext _context) : base(_context){}

    public async Task<ActorEntity> GetByName(string name)
    {
        if (name==null) throw new Exception("badrequest=name cannot  be null ");
        var actor = await _context.Actors.Include(x => x.Movies)
            .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        if (actor == null) throw new Exception("actor cannot be nulll");
        return actor;
    }

    public async Task<ActorEntity?> FindByName(string name)
    {
        return await _context.Actors.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
    }
}