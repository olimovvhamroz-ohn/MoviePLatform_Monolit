using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePlatformMonolit.Modules.Engagement.Application.CQRS.Views.Query;

public record GetTrendingQuery(int Days, int Take) : IRequest<List<TrendingMovieResponse>>;

public class GetTrendingQueryHandler(ApplicationDbContext context)
    : IRequestHandler<GetTrendingQuery, List<TrendingMovieResponse>>
{
    private const int MaxTake = 50;
    private const int MaxDays = 365;

    public async Task<List<TrendingMovieResponse>> Handle(GetTrendingQuery request, CancellationToken cancellationToken)
    {
        var days = Math.Clamp(request.Days <= 0 ? 7 : request.Days, 1, MaxDays);
        var take = Math.Clamp(request.Take <= 0 ? 10 : request.Take, 1, MaxTake);

        var since = DateTime.UtcNow.AddDays(-days);

        return await context.Views
            .AsNoTracking()
            .Where(v => v.LastWatchedAt >= since)
            .GroupBy(v => new { v.MovieId, v.Movie!.Title, v.Movie.PosterUrl })
            .Select(g => new TrendingMovieResponse
            {
                MovieId = g.Key.MovieId,
                MovieTitle = g.Key.Title,
                PosterUrl = g.Key.PosterUrl,
                Views = g.Count()
            })
            .OrderByDescending(x => x.Views)
            .ThenBy(x => x.MovieId)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}