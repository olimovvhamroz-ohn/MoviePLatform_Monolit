using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Entity;

namespace MoviePLatform_Monolit.Movie.Repositories;

public interface IReviewRepository:IBaseRepo<ReviewEntity>
{
    Task<List<MovieEntity>> GetTopMovie(int page, int pageSize);
    Task<List<ReviewEntity>> GetMovieReviews(long movieId,int page, int pageSize);
}
public class ReviewRepository:BaseRepository<ReviewEntity>,IReviewRepository
{
    public ReviewRepository (ApplicationDbContext context):base(context){}

    public async Task<List<ReviewEntity>> GetMovieReviews(long movieId, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var result = await _context.Reviews.
            Where(x => x.MovieId == movieId)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
        return result;
    }

    public async Task<List<MovieEntity>> GetTopMovie(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var topMovieIds=await _context.Reviews
            .GroupBy(r=>r.MovieId)
            .Select(g=>new{MovieId=g.Key,Avg=g.Average(t=>t.Rating)})
         .OrderByDescending(x => x.Avg)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(x => x.MovieId)
                    .ToListAsync();
        return await _context.Movies
            .Include(m => m.Category)
            .Where(m => topMovieIds.Contains(m.Id))
            .ToListAsync();
    }
}