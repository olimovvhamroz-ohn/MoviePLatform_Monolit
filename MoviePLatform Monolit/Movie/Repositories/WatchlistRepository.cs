using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Data;
using MoviePlatform_Monolit.Engagment.Entity;

namespace MoviePLatform_Monolit.Movie.Repositories;


public interface IWatchlistRepository
{
    Task<List<WatchlistEntity>> GetById(long id);
    Task<WatchlistEntity> Create(WatchlistEntity creat);
    Task<WatchlistEntity> IsWatched(long usrerId, long movieId);
    Task<bool> Exists(long userId, long movieId);
    Task<WatchlistEntity> Delete(long userid, long movieid);
}

public class WatchlistRepository : IWatchlistRepository
{
    private readonly ApplicationDbContext _context;

    public WatchlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WatchlistEntity>> GetById(long id)
    {
        return await _context.Watchlists.Where(x => x.UserId == id).ToListAsync();
    }

    public async Task<WatchlistEntity> Delete(long userid, long movieid)
    {
        var a = await _context.Watchlists.FirstOrDefaultAsync
            (x => x.UserId == userid && x.MovieId == movieid);
        if (a == null) throw new Exception("NotFoundExceptionwatchlist not found");
        _context.Remove(a);
        await _context.SaveChangesAsync();
        return a;
    }

    public async Task<WatchlistEntity> Create(WatchlistEntity create)
    {
        await _context.Set<WatchlistEntity>().AddAsync(create);
        await _context.SaveChangesAsync();
        return create;
    }

    public async Task<bool> Exists(long userId, long movieId)
    {
        return await _context.Watchlists
            .AnyAsync(x => x.MovieId == 
                movieId && x.UserId == userId);
    }

    public async Task<WatchlistEntity> IsWatched(long userid, long movieid)
    {
        var s = await _context.Watchlists.FirstOrDefaultAsync(x => x.MovieId == movieid && x.UserId == userid);
        if (s == null) throw new Exception("NotfoundException =not found");
        s.IsWatched=true;
        await _context.SaveChangesAsync();
        return s;
    }
    
}