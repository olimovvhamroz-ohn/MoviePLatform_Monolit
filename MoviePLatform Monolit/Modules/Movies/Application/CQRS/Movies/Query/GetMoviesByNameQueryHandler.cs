//using System.Text.Json;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesByNameQuery(string Title)
    : IRequest<List<MovieResponse>>;

public class GetMoviesByNameQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<GetMoviesByNameQueryHandler> logger
    //,IDistributedCache cache
)
    : IRequestHandler<GetMoviesByNameQuery, List<MovieResponse>>
{
    public async Task<List<MovieResponse>> Handle(
        GetMoviesByNameQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return new List<MovieResponse>();
        //var key = $"movie:name:{request.Title}";


        /*var cachedMovies = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            logger.LogInformation(
                "Movies with title {Title} found in Redis",
                request.Title);

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }*/

        logger.LogInformation(
            "Searching movies by title {Title} in database",
            request.Title);

        var movies =
            await context.Movies
                .AsNoTracking()
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Category)
                .Include(x => x.Reviews)
                .Include(x => x.Actors)
                .Include(x => x.Studio)
                .AsSplitQuery()
                .Where(x => x.Title.ToLower().Contains(request.Title.ToLower()))
                .ToListAsync(cancellationToken);

        // 3. Entity → Response
        var response = mapper.Map<List<MovieResponse>>(movies);

        /* // 4. Response → JSON
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
 */
        return response;
    }
}