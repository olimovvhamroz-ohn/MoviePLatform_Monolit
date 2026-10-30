using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Movies.Application.Projections;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesByNameQuery(string Title)
    : IRequest<List<MovieResponse>>;

public class GetMoviesByNameQueryHandler(
    ApplicationDbContext context,
    ILogger<GetMoviesByNameQueryHandler> logger,
    IDistributedCache cache)
    : IRequestHandler<GetMoviesByNameQuery, List<MovieResponse>>
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    public async Task<List<MovieResponse>> Handle(
        GetMoviesByNameQuery request,
        CancellationToken cancellationToken)
    {
        var term = request.Title?.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(term))
            return new List<MovieResponse>();

        var key = $"movie:name:{term}";

        var cachedMovies = await cache.GetStringAsync(key, cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            logger.LogInformation("Movies with title {Title} found in cache", term);
            return JsonSerializer.Deserialize<List<MovieResponse>>(cachedMovies)!;
        }

        logger.LogInformation("Searching movies by title {Title} in database", term);

        var movies = await context.Movies
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Title.ToLower().Contains(term))
            .OrderBy(x => x.Id)
            .Select(MovieProjections.ToResponse)
            .ToListAsync(cancellationToken);

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(movies),
            CacheOptions,
            cancellationToken);

        return movies;
    }
}