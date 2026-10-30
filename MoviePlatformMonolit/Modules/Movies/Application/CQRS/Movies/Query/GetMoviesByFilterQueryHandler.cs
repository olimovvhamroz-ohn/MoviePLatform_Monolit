using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Movies.Application.Projections;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Movies.Domain.Enums;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesByFilterQuery(
    long? CategoryId,
    int? Year,
    decimal? MaxPrice,
    AgeRating? AgeRating) : IRequest<List<MovieResponse>>;

public class GetMoviesByFilterQueryHandler(
    ApplicationDbContext context,
    ILogger<GetMoviesByFilterQueryHandler> logger,
    IDistributedCache cache)
    : IRequestHandler<GetMoviesByFilterQuery, List<MovieResponse>>
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public async Task<List<MovieResponse>> Handle(
        GetMoviesByFilterQuery request,
        CancellationToken cancellationToken)
    {

        var key = $"movies:filter:" +
                  $"category={request.CategoryId}:" +
                  $"year={request.Year}:" +
                  $"maxPrice={request.MaxPrice}:" +
                  $"age={request.AgeRating}";

        try
        {
            var cached = await cache.GetStringAsync(key, cancellationToken);
            if (cached is not null)
            {
                logger.LogInformation("Movies by filter found in cache");
                return JsonSerializer.Deserialize<List<MovieResponse>>(cached)!;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis недоступен, читаем из базы: {Key}", key);
        }

        logger.LogInformation("Getting movies by filter from database");

        var query = context.Movies.AsNoTracking();

        if (request.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);

        if (request.Year.HasValue)
            query = query.Where(x => x.Year == request.Year.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(x => x.Price <= request.MaxPrice.Value);

        if (request.AgeRating.HasValue)
            query = query.Where(x => x.AgeRating == request.AgeRating.Value);

        var result = await query
            .OrderBy(x => x.Id)
            .Select(MovieProjections.ToResponse)
            .ToListAsync(cancellationToken);

        try
        {
            await cache.SetStringAsync(key, JsonSerializer.Serialize(result), CacheOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Не удалось записать в кэш: {Key}", key);
        }
        return result;
    }

   
}