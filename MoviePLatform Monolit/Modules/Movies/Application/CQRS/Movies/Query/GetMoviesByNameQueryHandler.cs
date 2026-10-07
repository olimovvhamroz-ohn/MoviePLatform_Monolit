using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;

public record GetMoviesByNameQuery(string Title)
    : IRequest<List<MovieResponse>>;

public class GetMoviesByNameQueryHandler
    : IRequestHandler<GetMoviesByNameQuery, List<MovieResponse>>
{
    private readonly IMovieRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMoviesByNameQueryHandler> _logger;
    private readonly IDistributedCache _cache;

    public GetMoviesByNameQueryHandler(
        IMovieRepository repository,
        IMapper mapper,
        ILogger<GetMoviesByNameQueryHandler> logger,
        IDistributedCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<MovieResponse>> Handle(
        GetMoviesByNameQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"movie:name:{request.Title}";

        // 1. Check Redis
        var cachedMovies = await _cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            _logger.LogInformation(
                "Movies with title {Title} found in Redis",
                request.Title);

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }

        // 2. If not in Redis → PostgreSQL
        _logger.LogInformation(
            "Searching movies by title {Title} in database",
            request.Title);

        var movies = await _repository.GetMovieByNameAsync(
            request.Title,
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

        return response;
    }
}