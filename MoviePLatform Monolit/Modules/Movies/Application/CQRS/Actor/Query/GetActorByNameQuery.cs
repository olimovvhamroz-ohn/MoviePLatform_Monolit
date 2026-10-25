//using System.Text.Json;
using AutoMapper;
using MediatR;
//using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Extensions;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Actor.Query;

public record GetActorByNameQuery(string Name) : IRequest<ActorResponse>;

public class GetActorByNameQueryHandler(
    ApplicationDbContext context,
    IMapper mapper
    //,IDistributedCache cache
    ) : IRequestHandler<GetActorByNameQuery, ActorResponse>
{
    public async Task<ActorResponse> Handle(
        GetActorByNameQuery request,
        CancellationToken cancellationToken)
    {
       /* var cacheKey = $"actor:name:{request.Name}";

        var cachedActor = await _cache.GetStringAsync(
            cacheKey,
            cancellationToken);


        if (!string.IsNullOrEmpty(cachedActor))
        {
            return JsonSerializer.Deserialize<ActorResponse>(
                cachedActor)!;
        }
*/
        var actor = await context.Actors
            .Where(x=>x.IsDeleted==false)
            .Include(x => x.Movies)
            .FirstOrDefaultAsync(
                x => x.Name.ToLower() == request.Name.ToLower(),
                cancellationToken);
        if (actor == null)
            throw new NotFoundException($"Actor {request.Name} not found");
        
        var actorResponse = mapper.Map<ActorResponse>(actor);

/*
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
*/
        return actorResponse;
    }
}