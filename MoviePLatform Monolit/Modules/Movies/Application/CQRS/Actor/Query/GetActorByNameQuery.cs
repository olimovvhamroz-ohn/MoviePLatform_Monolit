using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Actor.Query;

public record GetActorByNameQuery(string Name) : IRequest<ActorResponse>;

public class GetActorByNameQueryHandler
    : IRequestHandler<GetActorByNameQuery, ActorResponse>
{
    private readonly IActorRepository _actorRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public GetActorByNameQueryHandler(
        IActorRepository actorRepository,
        IMapper mapper,
        IDistributedCache cache)
    {
        _actorRepository = actorRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ActorResponse> Handle(
        GetActorByNameQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"actor:name:{request.Name}";

        var cachedActor = await _cache.GetStringAsync(
            cacheKey,
            cancellationToken);

       
        if (!string.IsNullOrEmpty(cachedActor))
        {
            return JsonSerializer.Deserialize<ActorResponse>(
                cachedActor)!;
        }

        var actor = await _actorRepository.GetByName(request.Name);


      
        var actorResponse = _mapper.Map<ActorResponse>(actor);

        
        var json = JsonSerializer.Serialize(actorResponse);

     
        await _cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        return actorResponse;
    }
}