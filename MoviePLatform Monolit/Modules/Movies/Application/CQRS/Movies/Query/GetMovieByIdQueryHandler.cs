using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMovieByIdQuery(long Id) : IRequest<MovieResponse>;

public class GetMovieByIdQueryHandler(
    IMovieRepository repository,
    IMapper mapper,
    ILogger<GetMovieByIdQueryHandler> logger,
    IDistributedCache cache) : IRequestHandler<GetMovieByIdQuery, MovieResponse>
{
    public async Task<MovieResponse> Handle(
        GetMovieByIdQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"movie:{request.Id}";

        // 1. Check 
        var cachedMovie = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovie))
        {
            logger.LogInformation(
                "Movie {MovieId} found in Redis cache",
                request.Id);

            return JsonSerializer.Deserialize<MovieResponse>(
                cachedMovie)!;
        }

        // 2. If not in Redis → PostgreSQL
        logger.LogInformation(
            "Getting movie by id {MovieId} from database",
            request.Id);

        var movie = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        // 3. Map Entity → Response
        var response = mapper.Map<MovieResponse>(movie);

        // 4. Serialize Response → JSON
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

        logger.LogInformation(
            "Movie {MovieId} loaded from database and saved to Redis",
            request.Id);

        return response;
    }
}