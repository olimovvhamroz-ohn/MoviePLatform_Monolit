
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class ViewRepository : IViewRepository
{
    private readonly ApplicationDbContext _context;

    public ViewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ViewEntity> RecordView(long userId, long movieId, int positionSeconds, bool isCompleted)
    {
        var existing = await _context.Views
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);

        if (existing == null)
        {
            existing = new ViewEntity
            {
                UserId = userId,
                MovieId = movieId,
                PositionSeconds = positionSeconds,
                IsCompleted = isCompleted,
                FirstWatchedAt = DateTime.UtcNow,
                LastWatchedAt = DateTime.UtcNow,
                ViewCount = 1
            };
            await _context.Views.AddAsync(existing);
        }
        else
        {
            existing.PositionSeconds = positionSeconds;
            existing.IsCompleted = isCompleted;
            existing.LastWatchedAt = DateTime.UtcNow;
            existing.ViewCount += 1;
        }

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<List<ViewEntity>> GetHistory(long userId, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        return await _context.Views
            .Include(x => x.Movie)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.LastWatchedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<ViewEntity>> GetContinueWatching(long userId)
    {
        return await _context.Views
            .Include(x => x.Movie)
            .Where(x => x.UserId == userId && !x.IsCompleted && x.PositionSeconds > 0)
            .OrderByDescending(x => x.LastWatchedAt)
            .ToListAsync();
    }
    
   
   public async Task<List<(long MovieId, int Views)>> GetTrending(int days, int take)
   {
       var since = DateTime.UtcNow.AddDays(-days);

       var result = await _context.Views
           .Where(x => x.LastWatchedAt >= since)
           .GroupBy(x => x.MovieId)
           .Select(g => new
           {
               MovieId = g.Key,
               Views = g.Count()
           })
           .OrderByDescending(x => x.Views)
           .Take(take)
           .ToListAsync();

       return result
           .Select(x => (x.MovieId, x.Views))
           .ToList();
   }
   
   
   
   
   
   
   
   
   
   
   
   
   
   
}