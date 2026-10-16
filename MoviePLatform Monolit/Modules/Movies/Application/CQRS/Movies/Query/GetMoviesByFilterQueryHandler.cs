using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Enums;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesByFilterQuery(
    long? CategoryId,
    int? Year,
    decimal? MaxPrice,
    AgeRating? AgeRating) : IRequest<List<MovieResponse>>;

public class GetMoviesByFilterQueryHandler(
    IMovieRepository repository,
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

        var movies = await repository.GetMovieByFilterAsync(
            request.CategoryId,
            request.Year,
            request.MaxPrice,
            request.AgeRating,
            cancellationToken);
        
        var response = mapper.Map<List<MovieResponse>>(movies);
        
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
            "Movies loaded from database and saved to Redis");

        return response;
    }
}