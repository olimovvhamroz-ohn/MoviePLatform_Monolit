using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Actor.Command;
public record DeleteCommand(int id) : IRequest<ActorResponse>;
public class DeleteActorCommand:IRequestHandler<DeleteCommand,ActorResponse>
{
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteActorCommand> _logger;
    private readonly IActorRepository _actorRepository;
    private readonly IDistributedCache _cache;

    public DeleteActorCommand(
        IMapper mapper,
        ILogger<DeleteActorCommand> logger,
        IActorRepository actorRepository,
        IDistributedCache cache)
    {
        _actorRepository = actorRepository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }
    public async Task<ActorResponse> Handle(
        DeleteCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting delete actor with id {Id}",
            request.id);

        var deletedActor = await _actorRepository.DeleteAsync(request.id);

        await _cache.RemoveAsync(
            $"actor:{request.id}",
            cancellationToken);
        await _cache.RemoveAsync($"actor:{deletedActor.Name}", cancellationToken);

        return _mapper.Map<ActorResponse>(deletedActor);
    }
}