using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Command;

public record CreateWatchlistCommand(long UserId, long MovieId) : IRequest<WatchlistResponse>;

public class CreateWatchlistCommandHandler(
    ApplicationDbContext context,
    ILogger<CreateWatchlistCommandHandler> logger) : IRequestHandler<CreateWatchlistCommand, WatchlistResponse>
{
    public async Task<WatchlistResponse> Handle(CreateWatchlistCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        logger.LogInformation("Adding movie {MovieId} to watchlist for user {UserId}", request.MovieId, request.UserId);

        var movie = await context.Movies
            .AsNoTracking()
            .Where(m => m.Id == request.MovieId)
            .Select(m => new { m.Title, m.PosterUrl })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found");

        var exists = await context.Watchlists
            .AnyAsync(x => x.UserId == request.UserId && x.MovieId == request.MovieId, cancellationToken);

        if (exists)
            throw new BadRequestException("Movie is already in the watchlist");

        var entity = new WatchlistEntity
        {
            UserId = request.UserId,
            MovieId = request.MovieId
        };

        context.Watchlists.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new WatchlistResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            MovieId = entity.MovieId,
            MovieTitle = movie.Title,
            PosterUrl = movie.PosterUrl,
            IsWatched = entity.IsWatched,
            AddedAt = entity.AddedAt
        };
    }
}