using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Views.Command;

public record RecordViewCommand(
    long UserId,
    long MovieId,
    int PositionSeconds,
    bool IsCompleted) : IRequest<ViewResponse>;

public class RecordViewCommandHandler(
    ApplicationDbContext context,
    ILogger<RecordViewCommandHandler> logger) : IRequestHandler<RecordViewCommand, ViewResponse>
{
    public async Task<ViewResponse> Handle(RecordViewCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
            throw new BadRequestException("User ID must be greater than zero");

        if (request.MovieId <= 0)
            throw new BadRequestException("Movie ID must be greater than zero");

        if (request.PositionSeconds < 0)
            throw new BadRequestException("PositionSeconds cannot be negative");

        var movie = await context.Movies
            .AsNoTracking()
            .Where(x => x.Id == request.MovieId)
            .Select(x => new { x.Title, x.PosterUrl })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Movie with ID {request.MovieId} was not found");

        logger.LogInformation(
            "Recording view: user {UserId}, movie {MovieId}, position {Position}",
            request.UserId, request.MovieId, request.PositionSeconds);

        var view = await context.Views
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.MovieId == request.MovieId, cancellationToken);

        var now = DateTime.UtcNow;

        if (view == null)
        {
            view = new ViewEntity
            {
                UserId = request.UserId,
                MovieId = request.MovieId,
                PositionSeconds = request.PositionSeconds,
                IsCompleted = request.IsCompleted,
                FirstWatchedAt = now,
                LastWatchedAt = now,
                ViewCount = 1
            };
            context.Views.Add(view);
        }
        else
        {
            view.PositionSeconds = request.PositionSeconds;
            view.IsCompleted = request.IsCompleted;
            view.LastWatchedAt = now;
           
        }

        await context.SaveChangesAsync(cancellationToken);

        return new ViewResponse
        {
            Id = view.Id,
            UserId = view.UserId,
            MovieId = view.MovieId,
            MovieTitle = movie.Title,
            PosterUrl = movie.PosterUrl,
            PositionSeconds = view.PositionSeconds,
            IsCompleted = view.IsCompleted,
            ViewCount = view.ViewCount,
            LastWatchedAt = view.LastWatchedAt
        };
    }
}