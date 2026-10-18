using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Command;

public record DeleteMovieCommand(long Id) : IRequest<bool>;

public class DeleteMovieCommandHandler(
    ApplicationDbContext context,
    ILogger<DeleteMovieCommandHandler> logger,
    IDistributedCache cache)
    : IRequestHandler<DeleteMovieCommand, bool>
{
    public async Task<bool> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new BadRequestException("ID must be greater than zero");

        logger.LogInformation("Deleting movie {Id} via CQRS", request.Id);

        var movie = await context.Movies
                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                    ?? throw new NotFoundException($"Movie with ID {request.Id} was not found");

        context.Movies.Remove(movie);
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync($"movie:{request.Id}", cancellationToken);

        return true;
    }
}