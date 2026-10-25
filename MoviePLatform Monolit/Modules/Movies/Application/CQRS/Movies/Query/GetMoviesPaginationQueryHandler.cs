//using System.Text.Json;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesPaginationQuery(
    int Page,
    int PageSize) : IRequest<List<MovieResponse>>;

public class GetMoviesPaginationQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<GetMoviesPaginationQueryHandler> logger
    //,IDistributedCache cache
)
    : IRequestHandler<GetMoviesPaginationQuery, List<MovieResponse>>
{
    public async Task<List<MovieResponse>> Handle(
        GetMoviesPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 100);

        var skip = (request.Page - 1) * request.PageSize;

        /*var key =
            $"movie:page:{page}:size:{pageSize}";

        var cachedMovies = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            logger.LogInformation(
                "Movies page {Page} size {PageSize} found in Redis",
                request.Page,
                request.PageSize);

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }*/

        logger.LogInformation(
            "Getting movies page {Page} size {PageSize} from database",
            request.Page,
            request.PageSize);

        var movies = await context.Movies
            .AsNoTracking()
            .Where(x => x.IsDeleted == false)
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .Include(x => x.Actors)
            .Include(x => x.Studio)
            .AsSplitQuery()
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = mapper.Map<List<MovieResponse>>(movies);

        /*var json = JsonSerializer.Serialize(response);

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