

using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Extensions;

public record CreateWatchlistCommand(long UserId, long MovieId) : IRequest<WatchlistResponse>;

public class CreateWatchlistCommandHandler : IRequestHandler<CreateWatchlistCommand, WatchlistResponse>
{
    private readonly IWatchlistRepository _repository;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CreateWatchlistCommandHandler> _logger;

    public CreateWatchlistCommandHandler(IWatchlistRepository repository, ApplicationDbContext db, ILogger<CreateWatchlistCommandHandler> logger)
    {
        _repository = repository;
        _db = db;
        _logger = logger;
    }

    public async Task<WatchlistResponse> Handle(CreateWatchlistCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding movie {MovieId} to watchlist for user {UserId}", request.MovieId, request.UserId);

        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == request.MovieId, cancellationToken);
        if (movie == null)
            throw new NotFoundException($"Movie {request.MovieId} not found");

        var exists = await _repository.Exists(request.UserId, request.MovieId);
        if (exists)
            throw new BadRequestException("Movie is already in the watchlist");

        var entity = new WatchlistEntity { UserId = request.UserId, MovieId = request.MovieId };
        var created = await _repository.Create(entity);

        return new WatchlistResponse
            
        {
            Id = created.Id,
            UserId = created.UserId,
            MovieId = created.MovieId,
            MovieTitle = movie.Title,
            PosterUrl = movie.PosterUrl,
            IsWatched = created.IsWatched,
            AddedAt = created.AddedAt
        };
    }
}