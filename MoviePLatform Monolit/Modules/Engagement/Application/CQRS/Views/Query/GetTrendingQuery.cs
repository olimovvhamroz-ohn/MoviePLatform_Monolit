using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Views.Query;

public record GetTrendingQuery(int Days, int Take) : IRequest<List<TrendingMovieResponse>>;

public class GetTrendingQueryHandler(IViewRepository repository, ApplicationDbContext db) : IRequestHandler<GetTrendingQuery, List<TrendingMovieResponse>>
{

    public async Task<List<TrendingMovieResponse>> Handle(GetTrendingQuery request, CancellationToken cancellationToken)
    {
        var days = request.Days <= 0 ? 7 : request.Days;
        var take = request.Take <= 0 ? 10 : request.Take;

        var trending = await repository.GetTrending(days, take);
        var movieIds = trending.Select(x => x.MovieId).ToList();

        var movies = await db.Movies
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