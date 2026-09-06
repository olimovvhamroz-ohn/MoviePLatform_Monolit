using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Actor.Command;
public record DeleteCommand(int id) : IRequest<ActorResponse>;
public class DeleteActorCommand:IRequestHandler<DeleteCommand,ActorResponse>
{
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteActorCommand> _logger;
    private readonly IActorRepository _actorRepository;

    public DeleteActorCommand(IMapper maper, ILogger<DeleteActorCommand> logger, IActorRepository actorRepository)
    {
        _actorRepository = actorRepository;
        _mapper = maper;
        _logger = logger;
    }

    public async Task<ActorResponse> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("staring metod delete po {id} id");
        var delete = _actorRepository.DeleteAsync(request.id);
        return _mapper.Map<ActorResponse>(delete);
    }
}