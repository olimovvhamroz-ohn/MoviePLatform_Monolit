using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Entity.Enums;

public record GetMoviesByFilterQuery(
    long? CategoryId,
    int? Year,
    decimal? MaxPrice,
    AgeRating? AgeRating) : IRequest<List<MovieResponse>>;

public class GetMoviesByFilterQueryHandler
    : IRequestHandler<GetMoviesByFilterQuery, List<MovieResponse>>
{
    private readonly IMovieRepository _repository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetMoviesByFilterQueryHandler> _logger;

    public GetMoviesByFilterQueryHandler(
        IMovieRepository repository,
        IMapper mapper,
        IDistributedCache cache,
        ILogger<GetMoviesByFilterQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

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

        // 1. Check Redis
        var cachedMovies = await _cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            _logger.LogInformation(
                "Movies found in Redis cache for filter");

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }

        // 2. If Redis is empty → PostgreSQL
        _logger.LogInformation(
            "Getting movies from database by filter");

        var movies = await _repository.GetMovieByFilterAsync(
            request.CategoryId,
            request.Year,
            request.MaxPrice,
            request.AgeRating,
            cancellationToken);

        // 3. Entity → Response
        var response = _mapper.Map<List<MovieResponse>>(movies);

        // 4. Response → JSON
        var json = JsonSerializer.Serialize(response);

        // 5. Save to Redis
        await _cache.SetStringAsync(
            key,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        _logger.LogInformation(
            "Movies loaded from database and saved to Redis");

        return response;
    }
}