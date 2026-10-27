using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;

namespace MoviePlatformMonolit.Modules.Engagement.Application.CQRS.Views.Command;

public record ClearAllHistoryCommand(long UserId) : IRequest<bool>;

public class ClearAllHistoryCommandHandler(
    ApplicationDbContext context,
    ILogger<ClearAllHistoryCommandHandler> logger)
    : IRequestHandler<ClearAllHistoryCommand, bool>
{
    public async Task<bool> Handle(ClearAllHistoryCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
            throw new BadRequestException("UserId must be greater than zero");

        var user = await context.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken);

        if (!user) throw new NotFoundException($" UserId with ID {request.UserId} was not found");

        var view = await context.Views.Where(x => x.UserId == request.UserId).ToListAsync(cancellationToken);

        if (view.Count == 0)
            
        {
            logger.LogInformation("User {UserId} has no history to clear. Skipping database operation.", request.UserId);
            return true;
        }

        context.RemoveRange(view);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully cleared all history for user {UserId}. Removed {Count} view records.", request.UserId, view.Count);

        return true;
    }
}