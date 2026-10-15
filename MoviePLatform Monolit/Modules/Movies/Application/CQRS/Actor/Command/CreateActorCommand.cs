using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Actor.Command;

public record CreateCommand(ActorRequest ActorRequest) : IRequest<ActorResponse>;

public class CreateActorCommand (IMapper mapper, IActorRepository actorRepository, ILogger<CreateActorCommand> logger): IRequestHandler<CreateCommand, ActorResponse>
{
    
    public async Task<ActorResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating Actor {Name}", request.ActorRequest.Name);
        
        var dublicate = await actorRepository.FindByName(request.ActorRequest.Name);
        
        if (dublicate != null) throw new BadRequestException("Actor already exists");
        
        var model = mapper.Map<ActorEntity>(request.ActorRequest);
        
        var res = await actorRepository.CreateAsync(model, cancellationToken);
        
        return mapper.Map<ActorResponse>(res);
    }
}