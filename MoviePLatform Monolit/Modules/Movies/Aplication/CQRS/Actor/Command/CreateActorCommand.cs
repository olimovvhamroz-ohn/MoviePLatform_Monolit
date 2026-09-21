using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;
using UserService.Domain.Extensions;

namespace MoviePLatform_Monolit.Movie.CQRS.Actor.Command;

public record CreateCommand(ActorRequest ActorRequest) : IRequest<ActorResponse>;
public class CreateActorCommand:IRequestHandler<CreateCommand,ActorResponse>
{
    private readonly IMapper _mapper;
    private readonly IActorRepository _actorRepository;
    private readonly ILogger<CreateActorCommand> _logger;

    public CreateActorCommand(IMapper mapper, IActorRepository actorRepository, ILogger<CreateActorCommand> logger)
    {
        _mapper = mapper;
        _actorRepository = actorRepository;
        _logger = logger;
    }

    public async Task<ActorResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating Actor {Name}", request.ActorRequest.Name);     
        var dublicate=await _actorRepository.FindByName(request.ActorRequest.Name);
        if(dublicate !=null)throw new BadRequestException("Actor already exists");
        var model=_mapper.Map<ActorEntity>(request.ActorRequest);
        var res=await _actorRepository.CreateAsync(model,cancellationToken);
        return _mapper.Map<ActorResponse>(res);
    }
}