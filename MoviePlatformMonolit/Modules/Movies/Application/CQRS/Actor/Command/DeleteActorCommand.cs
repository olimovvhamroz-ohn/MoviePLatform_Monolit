using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Actor.Command;

public record DeleteCommand(int Id) : IRequest<Unit>;

public class DeleteActorCommandHandler(
    ApplicationDbContext context,
    ILogger<DeleteActorCommandHandler> logger)
    : IRequestHandler<DeleteCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new BadRequestException("ID must be greater than zero");

        logger.LogInformation("Deleting actor {Id}", request.Id);

        var entity = await context.Actors
                         .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.Id, cancellationToken)
                     ?? throw new NotFoundException($"Actor with id {request.Id} not found");

        entity.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}