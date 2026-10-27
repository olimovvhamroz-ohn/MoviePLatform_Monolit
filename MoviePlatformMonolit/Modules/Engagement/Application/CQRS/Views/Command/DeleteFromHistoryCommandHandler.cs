using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;

namespace MoviePlatformMonolit.Modules.Engagement.Application.CQRS.Views.Command;

public record DeleteFromHistoryCommand(long UserId, long ViewId) : IRequest<bool>;

public class DeleteFromHistoryCommandHandler(
    ApplicationDbContext context,
    ILogger<DeleteFromHistoryCommandHandler> logger)
    : IRequestHandler<DeleteFromHistoryCommand, bool>
{
    public async Task<bool> Handle(DeleteFromHistoryCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
            throw new BadRequestException("UserId ID must be greater than zero");

        if (request.ViewId <= 0)
            throw new BadRequestException("ViewId ID must be greater than zero");

        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                      ?? throw new NotFoundException($" UserId with ID {request.UserId} was not found");

        logger.LogInformation("Removing view :user {UserId}, from Views {ViewId}", user.Id,request.ViewId);


        var view = await context.Views.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Id == request.ViewId,
            cancellationToken)??throw new NotFoundException("View not found");


        context.Remove(view);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}