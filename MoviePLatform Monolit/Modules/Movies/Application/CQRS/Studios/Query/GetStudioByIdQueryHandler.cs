using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Studios.Query;

public record GetStudioByIdQuery(int Id) : IRequest<StudioResponse>;

public class GetStudioByIdQueryHandler
    : IRequestHandler<GetStudioByIdQuery, StudioResponse>
{
    private readonly IStudioRepository _repository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetStudioByIdQueryHandler> _logger;

    public GetStudioByIdQueryHandler(
        IStudioRepository repository,
        IMapper mapper,
        IDistributedCache cache,
        ILogger<GetStudioByIdQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<StudioResponse> Handle(
        GetStudioByIdQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"studio:{request.Id}";

        // 1. Check Redis
        var cachedStudio = await _cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedStudio))
        {
            _logger.LogInformation(
                "Studio {Id} found in Redis cache",
                request.Id);

            return JsonSerializer.Deserialize<StudioResponse>(
                cachedStudio)!;
        }

        // 2. Redis empty → PostgreSQL
        _logger.LogInformation(
            "Getting studio {Id} from database",
            request.Id);

        var studio = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        // 3. Entity → Response
        var response = _mapper.Map<StudioResponse>(studio);

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