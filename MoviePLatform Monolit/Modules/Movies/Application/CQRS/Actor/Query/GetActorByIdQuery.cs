using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Actor.Query;

public record GetByIdActorQuery(int ActorId) : IRequest<ActorResponse>;                                         

public class GetActorByIdQueryHandler : IRequestHandler<GetByIdActorQuery, ActorResponse>
{
    private readonly IActorRepository _actorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetActorByIdQueryHandler> _logger;
    private readonly IDistributedCache _cache;                                  

    public GetActorByIdQueryHandler(IMapper mapper, IActorRepository actorRepository,
        ILogger<GetActorByIdQueryHandler> logger, IDistributedCache cache)
    {
        _actorRepository = actorRepository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ActorResponse> Handle(
        GetByIdActorQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Калиди Redis
        var cacheKey = $"actor:{request.ActorId}";

        // 2. Аввал Redis-ро месанҷем
        var cachedActor = await _cache.GetStringAsync(
            cacheKey,
            cancellationToken);

        // 3. Агар дар Redis бошад → дигар ба database намеравем
        if (!string.IsNullOrEmpty(cachedActor))
        {
            _logger.LogInformation(
                "Actor {ActorId} found in Redis cache",
                request.ActorId);

            return JsonSerializer.Deserialize<ActorResponse>(cachedActor)!;
        }

        // 4. Агар Redis надошта бошад → PostgreSQL
        var actor = await _actorRepository.GetByIdAsync(request.ActorId);

        // 5. Entity → Response
        var actorResponse = _mapper.Map<ActorResponse>(actor);

        // 6. Response → JSON
        var json = JsonSerializer.Serialize(actorResponse);

        // 7. Дар Redis нигоҳ медорем
        await _cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        _logger.LogInformation(
            "Actor {ActorId} loaded from database and saved to Redis",
            request.ActorId);
        

        // 8. Натиҷа
        return actorResponse;
    }
}