using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Actor.Command;

public record DeleteCommand(int Id) : IRequest<ActorResponse>;

public class DeleteActorCommand(
    IMapper mapper,
    ILogger<DeleteActorCommand> logger,
    ApplicationDbContext context,
    IDistributedCache cache) : IRequestHandler<DeleteCommand, ActorResponse>
{
    public async Task<ActorResponse> Handle(
        DeleteCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id <= 0) throw new BadRequestException("ID must be greater than zero");

        logger.LogInformation(
            "Starting delete actor with id {Id}",
            request.Id);

        var entity = await context.Actors.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                     ?? throw new NotFoundException($"Actor with id {request.Id} not found");
        context.Actors.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(
            $"actor:{request.Id}",
            cancellationToken);
        
        await cache.RemoveAsync($"actor:{entity.Name}", cancellationToken);

        return mapper.Map<ActorResponse>(entity);
    }
}