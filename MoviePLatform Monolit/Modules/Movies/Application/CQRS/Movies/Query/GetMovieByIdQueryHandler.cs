using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;

public record GetMovieByIdQuery(long Id) : IRequest<MovieResponse>;

public class GetMovieByIdQueryHandler
    : IRequestHandler<GetMovieByIdQuery, MovieResponse>
{
    private readonly IMovieRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMovieByIdQueryHandler> _logger;
    private readonly IDistributedCache _cache;

    public GetMovieByIdQueryHandler(
        IMovieRepository repository,
        IMapper mapper,
        ILogger<GetMovieByIdQueryHandler> logger,
        IDistributedCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<MovieResponse> Handle(
        GetMovieByIdQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"movie:{request.Id}";

        // 1. Check Redis
        var cachedMovie = await _cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovie))
        {
            _logger.LogInformation(
                "Movie {MovieId} found in Redis cache",
                request.Id);

            return JsonSerializer.Deserialize<MovieResponse>(
                cachedMovie)!;
        }

        // 2. If not in Redis → PostgreSQL
        _logger.LogInformation(
            "Getting movie by id {MovieId} from database",
            request.Id);

        var movie = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        // 3. Map Entity → Response
        var response = _mapper.Map<MovieResponse>(movie);

        // 4. Serialize Response → JSON
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
            "Movie {MovieId} loaded from database and saved to Redis",
            request.Id);

        return response;
    }
}