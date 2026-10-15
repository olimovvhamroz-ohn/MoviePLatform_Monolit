using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Actor.Command;

public record DeleteCommand(int Id) : IRequest<ActorResponse>;

public class DeleteActorCommand(
    IMapper mapper,
    ILogger<DeleteActorCommand> logger,
    IActorRepository actorRepository,
    IDistributedCache cache) : IRequestHandler<DeleteCommand, ActorResponse>
{
    public async Task<ActorResponse> Handle(
        DeleteCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Starting delete actor with id {Id}",
            request.Id);

        var deletedActor = await actorRepository.DeleteAsync(request.Id);

        await cache.RemoveAsync(
            $"actor:{request.Id}",
            cancellationToken);
        await cache.RemoveAsync($"actor:{deletedActor.Name}", cancellationToken);

        return mapper.Map<ActorResponse>(deletedActor);
    }
}