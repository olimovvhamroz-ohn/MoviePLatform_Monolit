//using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMovieByIdQuery(long Id) : IRequest<MovieResponse>;

public class GetMovieByIdQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<GetMovieByIdQueryHandler> logger
    //,IDistributedCache cache
    ) : IRequestHandler<GetMovieByIdQuery, MovieResponse>
{
    public async Task<MovieResponse> Handle(
        GetMovieByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            throw new ArgumentException("ID must be greater than zero.", nameof(request.Id));
        }

        /*var key = $"movie:{request.Id}";

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
*/
        logger.LogInformation(
            "Getting movie by id {MovieId} from database",
            request.Id);


        var movie = await context.Movies
            .Where(x=>x.IsDeleted==false)
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .Include(x => x.Actors)
            .Include(x => x.Studio)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (movie == null)
            throw new KeyNotFoundException($"Movie with ID {request.Id} was not found.");


        // 3. Map Entity → Response
        var response = mapper.Map<MovieResponse>(movie);
/*
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
*/
        return response;
    }
}