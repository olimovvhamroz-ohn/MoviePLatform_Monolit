using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Engagement.Infrastructure.Repositories;

public class WatchlistRepository(ApplicationDbContext context) : IWatchlistRepository
{
   
    public async Task<List<WatchlistEntity>> GetByUserId(long userId)
    {
        return await context.Watchlists
            .Include(x => x.Movie)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AddedAt)
            .ToListAsync();
    }

    public async Task<WatchlistEntity> Create(WatchlistEntity entity)
    {
        await context.Watchlists.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Exists(long userId, long movieId)
    {
        return await context.Watchlists.AnyAsync(x => x.UserId == userId && x.MovieId == movieId);
    }

    public async Task<WatchlistEntity> MarkWatched(long userId, long movieId)
    {
        var entity = await context.Watchlists
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);

        if (entity == null) throw new NotFoundException("Watchlist entry not found");

        entity.IsWatched = true;
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<WatchlistEntity> Delete(long userId, long movieId)
    {
        var entity = await context.Watchlists
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);

        if (entity == null) throw new NotFoundException("Watchlist entry not found");

        context.Watchlists.Remove(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<int> CountByUserId(long userId)
    {
        return await context.Watchlists.CountAsync(x => x.UserId == userId);
    }
}