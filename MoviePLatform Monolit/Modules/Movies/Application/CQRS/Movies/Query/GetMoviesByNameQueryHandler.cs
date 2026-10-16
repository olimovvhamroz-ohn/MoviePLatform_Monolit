using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesByNameQuery(string Title)
    : IRequest<List<MovieResponse>>;

public class GetMoviesByNameQueryHandler(
    IMovieRepository repository,
    IMapper mapper,
    ILogger<GetMoviesByNameQueryHandler> logger,
    IDistributedCache cache)
    : IRequestHandler<GetMoviesByNameQuery, List<MovieResponse>>
{
    public async Task<List<MovieResponse>> Handle(
        GetMoviesByNameQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"movie:name:{request.Title}";

        // 1. Check Redis
        var cachedMovies = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            logger.LogInformation(
                "Movies with title {Title} found in Redis",
                request.Title);

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }

        // 2. If not in Redis → PostgreSQL
        logger.LogInformation(
            "Searching movies by title {Title} in database",
            request.Title);

        var movies = await repository.GetMovieByNameAsync(
            request.Title,
            cancellationToken);

        // 3. Entity → Response
        var response = mapper.Map<List<MovieResponse>>(movies);

        // 4. Response → JSON
        var json = JsonSerializer.Serialize(response);

        // 5. Save to Redis
        await cache.SetStringAsync(
            key,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        return response;
    }
}