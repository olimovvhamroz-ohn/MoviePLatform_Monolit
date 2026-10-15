using MediatR;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Views.Command;

public record RecordViewCommand(
    long UserId, 
    long MovieId, 
    int PositionSeconds, 
    bool IsCompleted) : IRequest<ViewResponse>;

public class RecordViewCommandHandler : IRequestHandler<RecordViewCommand, ViewResponse>
{
    private readonly IViewRepository _viewRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly ILogger<RecordViewCommandHandler> _logger;

    public RecordViewCommandHandler(
        IViewRepository viewRepository, 
        IMovieRepository movieRepository, 
        ILogger<RecordViewCommandHandler> logger)
    {
        _viewRepository = viewRepository;
        _movieRepository = movieRepository;
        _logger = logger;
    }

    public async Task<ViewResponse> Handle(RecordViewCommand request, CancellationToken cancellationToken)
    {
        if (request.PositionSeconds < 0)
            throw new BadRequestException("PositionSeconds cannot be negative.");

        var movie = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken);

        _logger.LogInformation(
            "Recording view: user {UserId}, movie {MovieId}, position {Position}",
            request.UserId, request.MovieId, request.PositionSeconds);

        var entity = await _viewRepository.RecordView(
            request.UserId, 
            request.MovieId, 
            request.PositionSeconds, 
            request.IsCompleted);

        return new ViewResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            MovieId = entity.MovieId,
            MovieTitle = movie.Title,
            PosterUrl = movie.PosterUrl,
            PositionSeconds = entity.PositionSeconds,
            IsCompleted = entity.IsCompleted,
            ViewCount = entity.ViewCount,
            LastWatchedAt = entity.LastWatchedAt
        };
    }
}