using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Enums;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesByFilterQuery(
    long? CategoryId,
    int? Year,
    decimal? MaxPrice,
    AgeRating? AgeRating) : IRequest<List<MovieResponse>>;

public class GetMoviesByFilterQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    IDistributedCache cache,
    ILogger<GetMoviesByFilterQueryHandler> logger)
    : IRequestHandler<GetMoviesByFilterQuery, List<MovieResponse>>
{
    public async Task<List<MovieResponse>> Handle(
        GetMoviesByFilterQuery request,
        CancellationToken cancellationToken)
    {
        
        var key =
            $"movie:filter:" +
            $"category={request.CategoryId}:" +
            $"year={request.Year}:" +
            $"maxPrice={request.MaxPrice}:" +
            $"ageRating={request.AgeRating}";


        var cachedMovies = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            logger.LogInformation(
                "Movies found in Redis cache for filter");

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }

        // 2. If Redis is empty → PostgreSQL
        logger.LogInformation(
            "Getting movies from database by filter");
        var query = context.Movies
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .Include(x => x.Actors)
            .Include(x => x.Studio)
            .AsSplitQuery();

        if (request.CategoryId.HasValue) 
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);

        if (request.Year.HasValue) 
            query = query.Where(x => x.Year == request.Year.Value); 

        if (request.MaxPrice.HasValue) 
            query = query.Where(x => x.Price <= request.MaxPrice.Value);

        if (request.AgeRating.HasValue) 
            query = query.Where(x => x.AgeRating == request.AgeRating.Value);

        var movies= await query.ToListAsync(cancellationToken);
        
        var response = mapper.Map<List<MovieResponse>>(movies);
        
        var json = JsonSerializer.Serialize(response);

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
            "Movies loaded from database and saved to Redis");

        return response;
    }
}