
using EngagementService.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record GetTrendingQuery(int Days, int Take) : IRequest<List<TrendingMovieResponse>>;

public class GetTrendingQueryHandler : IRequestHandler<GetTrendingQuery, List<TrendingMovieResponse>>
{
    private readonly IViewRepository _repository;
    private readonly ApplicationDbContext _db;

    public GetTrendingQueryHandler(IViewRepository repository, ApplicationDbContext db)
    {
        _repository = repository;
        _db = db;
    }

    public async Task<List<TrendingMovieResponse>> Handle(GetTrendingQuery request, CancellationToken cancellationToken)
    {
        var days = request.Days <= 0 ? 7 : request.Days;
        var take = request.Take <= 0 ? 10 : request.Take;

        var trending = await _repository.GetTrending(days, take);
        var movieIds = trending.Select(x => x.MovieId).ToList();

        var movies = await _db.Movies
            .Where(m => movieIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id, cancellationToken);

        return trending.Select(x => new TrendingMovieResponse
        {
            MovieId = x.MovieId,
            MovieTitle = movies.TryGetValue(x.MovieId, out var m) ? m.Title : null,
            PosterUrl = movies.TryGetValue(x.MovieId, out var m2) ? m2.PosterUrl : null,
            Views = x.Views
        }).ToList();
    }
}