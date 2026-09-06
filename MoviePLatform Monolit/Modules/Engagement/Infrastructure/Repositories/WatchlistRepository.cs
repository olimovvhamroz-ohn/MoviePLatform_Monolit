

using EngagementService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Extensions;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly ApplicationDbContext _context;

    public WatchlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WatchlistEntity>> GetByUserId(long userId)
    {
        return await _context.Watchlists
            .Include(x => x.Movie)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AddedAt)
            .ToListAsync();
    }

    public async Task<WatchlistEntity> Create(WatchlistEntity entity)
    {
        await _context.Watchlists.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Exists(long userId, long movieId)
    {
        return await _context.Watchlists.AnyAsync(x => x.UserId == userId && x.MovieId == movieId);
    }

    public async Task<WatchlistEntity> MarkWatched(long userId, long movieId)
    {
        var entity = await _context.Watchlists
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);

        if (entity == null) throw new NotFoundException("Watchlist entry not found");

        entity.IsWatched = true;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<WatchlistEntity> Delete(long userId, long movieId)
    {
        var entity = await _context.Watchlists
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);

        if (entity == null) throw new NotFoundException("Watchlist entry not found");

        _context.Watchlists.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
