using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Movies.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Actor.Command;

public record CreateCommand(ActorRequest ActorRequest) : IRequest<ActorResponse>;

public class CreateActorCommandHandler(
    IMapper mapper,
    ApplicationDbContext context,
    ILogger<CreateActorCommandHandler> logger)
    : IRequestHandler<CreateCommand, ActorResponse>
{
    public async Task<ActorResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        if (request.ActorRequest is null)
            throw new BadRequestException("Entity can not be null");

        var name = request.ActorRequest.Name.Trim();
        if (string.IsNullOrEmpty(name))
            throw new BadRequestException("Name is required");

        var nameLower = name.ToLower();

        logger.LogInformation("Creating actor {Name}", name);

        var exists = await context.Actors
            .AnyAsync(x => !x.IsDeleted && x.Name.ToLower() == nameLower, cancellationToken);

        if (exists)
            throw new BadRequestException("Actor already exists");

        var model = mapper.Map<ActorEntity>(request.ActorRequest);
        model.Name = name;

        context.Actors.Add(model);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ActorResponse>(model);
    }
}