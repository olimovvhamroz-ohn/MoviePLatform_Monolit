
using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Application.Projections;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMovieByIdQuery(long Id) : IRequest<MovieResponse>;

public class GetMovieByIdQueryHandler(
    ApplicationDbContext context,
    ILogger<GetMovieByIdQueryHandler> logger,
    IDistributedCache cache
) : IRequestHandler<GetMovieByIdQuery, MovieResponse>
{
    public async Task<MovieResponse> Handle(
        GetMovieByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            throw new BadRequestException("ID must be greater than zero");
        }

        var key = $"movie:{request.Id}";


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

        
        logger.LogInformation(
            "Getting movie by id {MovieId} from database",
            request.Id);


        var movie = await context.Movies
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Id == request.Id)
            .Select(MovieProjections.ToResponse)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Movie with ID {request.Id} was not found");


        var json = JsonSerializer.Serialize(movie);

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
        return movie;
    }
}