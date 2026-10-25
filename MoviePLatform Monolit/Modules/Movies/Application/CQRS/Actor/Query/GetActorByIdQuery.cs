//using System.Text.Json;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Actor.Query;

public record GetByIdActorQuery(int ActorId) : IRequest<ActorResponse>;

public class GetActorByIdQueryHandler(
    IMapper mapper,
    ApplicationDbContext context,
    ILogger<GetActorByIdQueryHandler> logger
    //,IDistributedCache cache
) : IRequestHandler<GetByIdActorQuery, ActorResponse>
{
    public async Task<ActorResponse> Handle(
        GetByIdActorQuery request,
        CancellationToken cancellationToken)
    {
        if (request.ActorId <= 0) throw new BadRequestException("ID must be greater than zero");
        // var cacheKey = $"actor:{request.ActorId}";

        /*var cachedActor = await cache.GetStringAsync(
            cacheKey,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedActor))
        {
            logger.LogInformation(
                "Actor {ActorId} found in Redis cache",
                request.ActorId);

            return JsonSerializer.Deserialize<ActorResponse>(cachedActor)!;
        }
        */
        logger.LogInformation(
            "Actor {ActorId} found in Database ",
            request.ActorId);
        var actor = await context.Actors
            .AsNoTracking()
            .Where(x => x.IsDeleted == false)
            .Include(x => x.Movies)
            .FirstOrDefaultAsync(x =>
                x.Id == request.ActorId, cancellationToken);

        if (actor == null)
            throw new NotFoundException($"Id {request.ActorId} not found");

        var actorResponse = mapper.Map<ActorResponse>(actor);
/*
        var json = JsonSerializer.Serialize(actorResponse);

        await cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        logger.LogInformation(
            "Actor {ActorId} loaded from database and saved to Redis",
            request.ActorId);


        // 8. Натиҷа
        */
        return actorResponse;
    }
}