using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

public interface IActorRepository : IBaseRepo<ActorEntity>
{
    Task<ActorEntity>GetByName(string name);
    Task<ActorEntity?> FindByName(string name);
}
public class ActorRepository(ApplicationDbContext context) : BaseRepository<ActorEntity>(context), IActorRepository
{
  
    public async Task<ActorEntity> GetByName(string name)
    {
        if (name==null) throw new Exception("name cannot  be null ");
        
        var actor = await context.Actors.Include(x => x.Movies)
            .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        
        if (actor == null) throw new Exception("actor cannot be nulll");
        
        return actor;
    }

    public async Task<ActorEntity?> FindByName(string name)
    {
        return await context.Actors.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
    }
}