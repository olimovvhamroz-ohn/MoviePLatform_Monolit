using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Actor.Command;

public record CreateCommand(ActorRequest ActorRequest) : IRequest<ActorResponse>;

public class CreateActorCommand (IMapper mapper, ApplicationDbContext context, ILogger<CreateActorCommand> logger): IRequestHandler<CreateCommand, ActorResponse>
{
    
    public async Task<ActorResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        if (request.ActorRequest == null) throw new BadRequestException("Entity can not be null");

        logger.LogInformation("Creating Actor {Name}", request.ActorRequest.Name);
        
        var dublicate = await context.Actors.FirstOrDefaultAsync(x => x.Name.ToLower() == request.ActorRequest.Name .ToLower(), cancellationToken: cancellationToken);
        
        if (dublicate != null) throw new BadRequestException("Actor already exists");
        
        var model = mapper.Map<ActorEntity>(request.ActorRequest);
        
        await context.Actors.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        
        return mapper.Map<ActorResponse>(model);
    }
}